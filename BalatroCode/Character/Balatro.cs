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

    public static readonly SpireField<PlayerCombatState, int> CombatGoldEarned = new(() => 0);
    public static readonly SpireField<PlayerCombatState, int> CardsDiscardedThisTurn = new(() => 0);
    public static readonly SpireField<PlayerCombatState, int> CardsDiscardedThisCombat = new(() => 0);
    public static event Action<Player>? CombatGoldEarnedChanged;
    public static event Action? CombatStart;
    public static event Action? CardPlayed;
    

    public static readonly Color Color = new("ffffff");
    
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
        if (Stakes.CurrentStake() >= 1 && player.RunState.CurrentRoom?.RoomType is Treasure) return 0;
        
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
    
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (combatState.RoundNumber > 1 || side != CombatSide.Player) return;
        var roomType = combatState.RunState.CurrentRoom?.RoomType;
        if (roomType is Elite or Boss)
        {
            var enemy = combatState.Enemies.FirstOrDefault(creature =>
                creature is { IsPet: false, CanReceivePowers: true, IsPlayer: false });
            if (enemy != null)
            {
                var blindPowers = enemy.Powers.Where(p => p is IBlindPower).Select(p => ((IBlindPower)p).BlindType)
                    .ToList();
                await PowerCmd.Apply(choiceContext,
                    BlindMethods.GetRandomBlindPower(enemy, roomType == Boss, blindPowers).ToMutable(),
                    enemy, 1, null, null);
            }
        }
        if (Stakes.CurrentStake() <= 1) return;
        foreach (var creature in combatState.Enemies)
        {
            if (Stakes.CurrentStake() > 1)
            {
                var extraHp = creature.MaxHp + creature.CombatState?.RunState.TotalFloor * 2;
                if (extraHp != null)
                {
                    var hpDiff = creature.MaxHp - creature.CurrentHp;
                    creature.SetMaxHpInternal((decimal)extraHp);
                    creature.SetCurrentHpInternal((decimal)extraHp - hpDiff);
                }
            }
            if (Stakes.CurrentStake() > 4)
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
        if (Stakes.CurrentStake() < 4) return count;
        return player.PlayerCombatState?.TurnNumber > 1 ? count : count - 2;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (Stakes.CurrentStake() < 7) return;
        await PlayerCmd.LoseGold(2, cardPlay.Card.Owner);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatManager.Instance.History.CardPlaysFinished
            .Any(c => !c.CardPlay.IsAutoPlay && c.CardPlay.Card.Owner == cardPlay.Card.Owner)) CardPlayed?.Invoke();
        return base.AfterCardPlayed(choiceContext, cardPlay);
    }
    
    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (Stakes.CurrentStake() < 6) return base.AfterCombatEnd(room);

        var rewardsSetSynchronizer = Traverse.Create(RunManager.Instance).Property("RewardsSetSynchronizer").GetValue<RewardsSetSynchronizer>();
        var localPlayer = Traverse.Create(rewardsSetSynchronizer).Property("LocalPlayer").GetValue<Player>();
        var playerRewardState = Traverse.Create(rewardsSetSynchronizer).Method("GetRewardStateForPlayer", localPlayer).GetValue();
        var rewardsStack = Traverse.Create(playerRewardState).Field("rewardsStack").GetValue<System.Collections.IList>();
        _ = TaskHelper.RunSafely(ApplyPerishable(rewardsStack, localPlayer, room));
        return base.AfterCombatEnd(room);
    }
    
    private async Task ApplyPerishable(System.Collections.IList rewardsStack, Player player, CombatRoom room)
    {
        var attempts = 0;
        while (rewardsStack.Count == 0 && attempts++ < 30) await Cmd.Wait(1);
        if (rewardsStack.Count == 0) return;
        
        var perishable = ModelDb.Enchantment<Perishable>();
        
        foreach (var setStateObj in rewardsStack)
        {
            var set = Traverse.Create(setStateObj).Field("set").GetValue<RewardsSet>();
            if (set.Room != room) continue;
            foreach (var reward in set.Rewards)
            {
                if (reward is not CardReward cardReward) continue;
                foreach (var card in cardReward.Cards.Where(perishable.CanEnchant))
                {
                    MainFile.Logger.Info("test 4: " + card);
                    if (player.PlayerRng.Rewards.NextFloat() < 0.3) CardCmd.Enchant<Perishable>(card, 10);
                }
            }
        }
    }
    
    /*
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        var state = Traverse.Create(RunManager.Instance).Property("State").GetValue<RunState>();
        if (state == null || room is not (RestSiteRoom or EventRoom)) return base.AfterRoomEntered(room);
        {
            var counter = room is RestSiteRoom ? RestSitesVisitedThisAct : QuestionMarksVisited;
            foreach (var player in state.Players)
                if (player.Character == this)
                    counter.Set(player, counter.Get(player) + 1);
        }
        return base.AfterRoomEntered(room);
    }

    public override Task AfterActEntered()
    {
        var state = Traverse.Create(RunManager.Instance).Property("State").GetValue<RunState>();
        if (state == null) return base.AfterActEntered();
        foreach (var player in state.Players)
            if (player.Character == this)
                RestSitesVisitedThisAct.Set(player, 0);
        return base.AfterActEntered();
    }


    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.PlayerCombatState != null) CardsDiscardedThisTurn.Set(player.PlayerCombatState, 0);
        player.PlayerCombatState?.AllCards
            .OfType<IRandomType>()
            .ToList()
            .ForEach(card => card.SetRandomType());
        return base.AfterPlayerTurnStart(choiceContext, player);
    }

    public override Task BeforeCardRemoved(CardModel card)
    {
        CardsRemoved.Set(card.Owner, CardsRemoved.Get(card.Owner) + 1);
        return base.BeforeCardRemoved(card);
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        var pile = card.Pile;
        if (pile is { Type: PileType.Deck }) CardsAdded.Set(card.Owner, CardsAdded.Get(card.Owner) + 1);
        return base.AfterCardChangedPiles(card, oldPileType, clonedBy);
    }


    public override Task AfterPotionUsed(PotionModel potion, Creature? target)
    {
        PotionsUsed.Set(potion.Owner, PotionsUsed.Get(potion.Owner) + 1);
        return base.AfterPotionUsed(potion, target);
    }

    public override Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner.PlayerCombatState != null)
        {
            CardsDiscardedThisTurn.Set(card.Owner.PlayerCombatState,
                1 + CardsDiscardedThisTurn.Get(card.Owner.PlayerCombatState));
            CardsDiscardedThisCombat.Set(card.Owner.PlayerCombatState,
                1 + CardsDiscardedThisCombat.Get(card.Owner.PlayerCombatState));
        }

        return base.AfterCardDiscarded(choiceContext, card);
    }
    */
    
    public override NCreatureVisuals CreateCustomVisuals()
    {
        var visual = NodeFactory<NCreatureVisuals>.CreateFromResource($"decks/{BalatroConfig.SelectedDeck}.png".ImagePath());
        return visual;
    }
    public override string CustomMerchantAnimPath => "scenes/merchant.tscn".ImagePath();

    public override string CustomRestSiteAnimPath => "scenes/rest_site.tscn".ImagePath();
    
    //public override string CustomEnergyCounterPath { get; }

    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectBg => "char_select_bg_balatro.tscn".CharacterUiPath();
    public override Color MapDrawingColor => Color.Color8(100, 100, 100);
}