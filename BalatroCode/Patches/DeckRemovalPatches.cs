using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Patches;

public class DeckRemovalPatches
{
    [HarmonyPatch(typeof(CardSelectCmd), nameof(CardSelectCmd.FromDeckForRemoval))]
    public static class BalatroFromDeckForRemovalPatch
    {
        private static void Prefix(Player player, CardSelectorPrefs prefs, ref Func<CardModel, bool>? filter)
        {
            if (Stakes.CurrentStake(player) < 3) return;
            filter = c => c.Rarity != CardRarity.Basic;
        }
    }

    [HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.RemoveFromDeck), typeof(IReadOnlyList<CardModel>),
        typeof(bool))]
    public static class BalatroRemoveFromDeckPatch
    {
        [HarmonyPrefix]
        private static void Prefix(ref IReadOnlyList<CardModel> cards)
        {
            if (Stakes.CurrentStake(cards.FirstOrDefault()?.Owner) < 3) return;
            cards = cards.Where(c => c.Rarity != CardRarity.Basic && !c.Keywords.Contains(CardKeyword.Eternal))
                .ToList();
        }
    }
}