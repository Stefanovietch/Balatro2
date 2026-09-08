using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Enchantments;
using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Balatro.BalatroCode.Extensions;
using Balatro.BalatroCode.Patches;
using Balatro.BalatroCode.Powers;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using static MegaCrit.Sts2.Core.Rooms.RoomType;

namespace Balatro.BalatroCode.Character;

public class Balatro : PlaceholderCharacterModel
{
    public const string CharacterId = "Balatro";

    public static readonly SavedSpireField<Player, int> CardsRemoved = new(() => 0, "balatro_cards_removed");
    public static readonly SavedSpireField<Player, int> CardsAdded = new(() => 0, "balatro_cards_added");
    public static readonly SavedSpireField<Player, int> PotionsUsed = new(() => 0, "balatro_potions_used");

    public static readonly SavedSpireField<Player, int> RestSitesVisitedThisAct =
        new(() => 0, "rest_sites_visited_this_act");

    public static readonly SavedSpireField<Player, int> QuestionMarksVisited = new(() => 0, "question_marks_visited");
    public static readonly SavedSpireField<Player, int> MaxCombatGold = new(() => 200, "balatro_max_combat_gold");
    public static readonly SavedSpireField<Player, bool> GrosMichelExtinct = new(() => false, "balatro_gros_michel_extinct");

    public static readonly SpireField<PlayerCombatState, int> CombatGoldEarned = new(() => 0);
    public static readonly SpireField<PlayerCombatState, int> CardsDiscardedThisTurn = new(() => 0);
    public static readonly SpireField<PlayerCombatState, int> CardsDiscardedThisCombat = new(() => 0);
    public static event Action<Player>? CombatGoldEarnedChanged;
    public static event Action? CombatStart;
    public static event Action? CardPlayed;
    

    public static readonly Color Color = new("f0f0f0");
    
    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 70;
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<JollyJoker>(),
        ModelDb.Card<JollyJoker>(),
        ModelDb.Card<JollyJoker>(),
        ModelDb.Card<JollyJoker>(),
        ModelDb.Card<SlyJoker>(),
        ModelDb.Card<SlyJoker>(),
        ModelDb.Card<SlyJoker>(),
        ModelDb.Card<SlyJoker>(),
        ModelDb.Card<MadJoker>(),
        ModelDb.Card<CleverJoker>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<BurningBlood>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<BalatroCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<BalatroRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<BalatroPotionPool>();
    
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override decimal ModifyGoldGained(Player player, decimal amount)
    {
        if (Stakes.CurrentStake(player) >= 1 && player.RunState.CurrentRoom?.RoomType is Treasure) return 0;
        
        if (player.PlayerCombatState == null || 
            !BalatroConfig.GoldCap || 
            !HookPatches.ModifyGoldGainedScopePatch.ProcessedThisCall.Add(this)) //already run for this player
            return base.ModifyGoldGained(player, amount);
        
        var maxCombatGold = MaxCombatGold.Get(player);
        var earned = CombatGoldEarned.Get(player.PlayerCombatState);
        var remaining = maxCombatGold - earned;
        if (remaining <= 0) return base.ModifyGoldGained(player, 0);
        var goldToGain = Math.Min(amount, remaining);
        CombatGoldEarned.Set(player.PlayerCombatState, earned + (int)goldToGain);
        CombatGoldEarnedChanged?.Invoke(player);
        return base.ModifyGoldGained(player, goldToGain);
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.PlayerCombatState?.TurnNumber > 1 || player.Character is not Balatro || !LocalContext.IsMe(player)) return;
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;
        var roomType = combatState.RunState.CurrentRoom?.RoomType;
        if (roomType is Elite or Boss && BalatroConfig.BlindsActive)
        {
            var enemy = combatState.Enemies.FirstOrDefault(creature =>
                creature is { IsPet: false, CanReceivePowers: true, IsPlayer: false, IsPrimaryEnemy: true });
            if (enemy != null)
            {
                var blindPowers = enemy.Powers.Where(p => p is IBlindPower).Select(p => ((IBlindPower)p).BlindType)
                    .ToList();
                await PowerCmd.Apply(choiceContext,
                    BlindMethods.GetRandomBlindPower(enemy, roomType == Boss, blindPowers).ToMutable(),
                    enemy, 1, null, null);
            }
        }
        
        if (Stakes.CurrentStake(player) <= 1) return;
        foreach (var creature in combatState.Enemies)
        {
            if (Stakes.CurrentStake(player) > 1)
            {
                var extraHp = creature.MaxHp + creature.CombatState?.RunState.TotalFloor * 2;
                if (extraHp != null)
                {
                    var hpDiff = creature.MaxHp - creature.CurrentHp;
                    creature.SetMaxHpInternal((decimal)extraHp);
                    creature.SetCurrentHpInternal((decimal)extraHp - hpDiff);
                }
            }
            if (Stakes.CurrentStake(player) > 4)
            {
                decimal? amount = 1;
                var typeMult = creature.CombatState?.RunState.CurrentRoom?.RoomType;
                if (typeMult != null) amount = (creature.CombatState?.RunState.CurrentActIndex + 1) * (decimal) typeMult;
                await PowerCmd.Apply<TopUpPower>(new ThrowingPlayerChoiceContext(), creature, amount ?? 1, null, null);
            }
        }
    }
    
    public override Task BeforeCombatStart()
    {
        CombatStart?.Invoke();
        
        return base.BeforeCombatStart();
    }

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (Stakes.CurrentStake(player) < 4) return count;
        return player.PlayerCombatState?.TurnNumber > 1 ? count : count - 2;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (Stakes.CurrentStake(cardPlay.Card.Owner) < 7) return;
        await PlayerCmd.LoseGold(2, cardPlay.Card.Owner);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatManager.Instance.History.CardPlaysFinished
            .Any(c => !c.CardPlay.IsAutoPlay && c.CardPlay.Card.Owner == cardPlay.Card.Owner)) CardPlayed?.Invoke();
        return base.AfterCardPlayed(choiceContext, cardPlay);
    }
    
    public override NCreatureVisuals CreateCustomVisuals()
    {
        var visual = NodeFactory<NCreatureVisuals>.CreateFromResource($"decks/{BalatroConfig.SelectedDeck}.png".ImagePath());
        return visual;
    }
    public override string CustomMerchantAnimPath => "scenes/merchant.tscn".ImagePath();

    public override string CustomRestSiteAnimPath => "scenes/rest_site.tscn".ImagePath();
    
    public override string CustomEnergyCounterPath => "scenes/energy_counter.tscn".ImagePath();
    public override Color EnergyLabelOutlineColor => new (68/255f, 107/255f, 235/255f);
    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectBg => "char_select_bg_balatro.tscn".CharacterUiPath();
    public override Color MapDrawingColor => Color.Color8(100, 100, 100);
}