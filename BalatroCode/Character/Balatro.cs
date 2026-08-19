using Balatro.BalatroCode.Cards;
using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Balatro.BalatroCode.Extensions;
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
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
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

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
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
        if (player.PlayerCombatState == null || !BalatroConfig.GoldCap) return base.ModifyGoldGained(player, amount);
        var maxCombatGold = MaxCombatGold.Get(player);
        var earned = CombatGoldEarned.Get(player.PlayerCombatState);
        var remaining = maxCombatGold - earned;
        if (remaining <= 0) return base.ModifyGoldGained(player, 0);
        var goldToGain = Math.Min(amount, remaining);
        CombatGoldEarned.Set(player.PlayerCombatState, earned + (int)goldToGain);
        CombatGoldEarnedChanged?.Invoke(player);
        return base.ModifyGoldGained(player, goldToGain);
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

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (combatState.RoundNumber > 1 || side != CombatSide.Player) return;
        var roomType = combatState.RunState.CurrentRoom?.RoomType;
        if (roomType is not (Elite or Boss)) return;
        var enemy = combatState.Enemies.FirstOrDefault(creature =>
            creature is { IsPet: false, CanReceivePowers: true, IsPlayer: false });
        if (enemy == null) return;
        await PowerCmd.Apply(choiceContext, BlindMethods.GetRandomBlindPower(enemy, roomType == Boss).ToMutable(), enemy, 1, null,
            null);
    }
    
    public override NCreatureVisuals CreateCustomVisuals()
    {
        var visual = NodeFactory<NCreatureVisuals>.CreateFromResource("res://Balatro/images/decks/"+ BalatroConfig.SelectedDeck +".png");
        return visual;
    }

    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectBg => "char_select_bg_balatro.tscn".CharacterUiPath();
    public override Color MapDrawingColor => Color.Color8(100, 100, 100);
}