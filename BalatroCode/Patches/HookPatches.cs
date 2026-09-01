using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Enchantments;
using Balatro.BalatroCode.Powers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Rooms;

namespace Balatro.BalatroCode.Patches;

public class HookPatches
{
    [HarmonyPatch(typeof(Hook), nameof(Hook.ModifyGoldGained))]
    public static class ModifyGoldGainedScopePatch
    {
        public static readonly HashSet<AbstractModel> ProcessedThisCall = new();

        [HarmonyPrefix]
        static void Prefix() => ProcessedThisCall.Clear();
        [HarmonyPostfix]
        static void Postfix() => ProcessedThisCall.Clear();
    }
    
    [HarmonyPatch(typeof(Hook), nameof(Hook.AfterPlayerTurnStart))]
    public static class BalatroAfterPlayerTurnStartPatch
    {
        [HarmonyPrefix]
        static void Prefix(ICombatState combatState, PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Character is not Character.Balatro) return;
            
            if (player.PlayerCombatState != null)
                Character.Balatro.CardsDiscardedThisTurn.Set(player.PlayerCombatState, 0);

            player.PlayerCombatState?.AllCards
                .OfType<IRandomType>()
                .ToList()
                .ForEach(card => card.SetRandomType());
        }
    }
    
    [HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardDiscarded))]
    public static class BalatroAfterCardDiscardedPatch
    {
        [HarmonyPrefix]
        static void Prefix(CardModel card)
        {
            var owner = card.Owner;
            if (owner?.Character is not Character.Balatro) return;
            if (owner.PlayerCombatState == null) return;

            Character.Balatro.CardsDiscardedThisTurn.Set(owner.PlayerCombatState,
                1 + Character.Balatro.CardsDiscardedThisTurn.Get(owner.PlayerCombatState));
            Character.Balatro.CardsDiscardedThisCombat.Set(owner.PlayerCombatState,
                1 + Character.Balatro.CardsDiscardedThisCombat.Get(owner.PlayerCombatState));
        }
    }
    
    [HarmonyPatch(typeof(Hook), nameof(Hook.BeforeCardRemoved))]
    public static class BalatroBeforeCardRemovedPatch
    {
        [HarmonyPrefix]
        static void Prefix(CardModel card)
        {
            var owner = card.Owner;
            if (owner?.Character is not Character.Balatro) return;

            Character.Balatro.CardsRemoved.Set(owner, Character.Balatro.CardsRemoved.Get(owner) + 1);
        }
    }
    
    [HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardChangedPiles))]
    public static class BalatroAfterCardChangedPilesPatch
    {
        [HarmonyPrefix]
        static void Prefix(CardModel card)
        {
            var owner = card.Owner;
            if (owner?.Character is not Character.Balatro) return;
            if (card.Pile is not { Type: PileType.Deck }) return;

            Character.Balatro.CardsAdded.Set(owner, Character.Balatro.CardsAdded.Get(owner) + 1);
        }
    }
    
    [HarmonyPatch(typeof(Hook), nameof(Hook.AfterPotionUsed))]
    public static class BalatroAfterPotionUsedPatch
    {
        [HarmonyPrefix]
        static void Prefix(PotionModel potion)
        {
            var owner = potion.Owner;
            if (owner?.Character is not Character.Balatro) return;

            Character.Balatro.PotionsUsed.Set(owner, Character.Balatro.PotionsUsed.Get(owner) + 1);
        }
    }
    
    [HarmonyPatch(typeof(Hook), nameof(Hook.AfterRoomEntered))]
    public static class BalatroAfterRoomEnteredPatch
    {        
        [HarmonyPrefix]
        static void Prefix(IRunState runState, AbstractRoom room)
        {
            if (room is not RestSiteRoom) return;
            
            foreach (var player in runState.Players)
                if (player.Character is Character.Balatro) 
                    Character.Balatro.RestSitesVisitedThisAct.Set(player, Character.Balatro.RestSitesVisitedThisAct.Get(player) + 1);
        }
    }
    
    [HarmonyPatch(typeof(RunManager), nameof(RunManager.EnterMapPointInternal))]
    public static class BalatroEnterMapPointInternalPatch
    {        
        
        [HarmonyPrefix]
        static void Prefix(MapPointType pointType)
        {
            if (pointType is not MapPointType.Unknown) return;
            var runState = Traverse.Create(RunManager.Instance).Property("State").GetValue<RunState>();
            if (runState == null) return;
            foreach (var player in runState.Players)
                if (player.Character is Character.Balatro) 
                    Character.Balatro.QuestionMarksVisited.Set(player, Character.Balatro.QuestionMarksVisited.Get(player) + 1);
        }
    }
    
    [HarmonyPatch(typeof(Hook), nameof(Hook.AfterActEntered))]
    public static class BalatroAfterActEnteredPatch
    {
        [HarmonyPrefix]
        static void Prefix(IRunState runState)
        {
            foreach (var player in runState.Players)
                if (player.Character is Character.Balatro) 
                    Character.Balatro.RestSitesVisitedThisAct.Set(player, 0);
                
        }
    }
    
    [HarmonyPatch(typeof(Hook), nameof(Hook.TryModifyCardRewardOptions))]
    public static class BalatroTryModifyCardRewardOptionsPatch
    {
        [HarmonyPostfix]
        static void Postfix(IRunState runState, Player player, List<CardCreationResult> cardRewardOptions,
            CardCreationOptions creationOptions)
        {
            if (Stakes.CurrentStake(player) < 6) return;
            foreach (var card in cardRewardOptions.Select(r => r.Card).Where(c => c.Enchantment == null && ModelDb.Enchantment<Perishable>().CanEnchant(c)))
            {
                if (player.PlayerRng.Rewards.NextFloat() < 0.3) CardCmd.Enchant<Perishable>(card, 10);
            }
        }
    }
}