using Balatro.BalatroCode.Powers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Patches;

public class ShouldDiscardPatches
{
    [HarmonyPatch(typeof(CardCmd), nameof(CardCmd.DiscardAndDraw))]
    public static class DiscardAndDrawPatch
    {
        static void Prefix(ref IEnumerable<CardModel> cardsToDiscard)
        {
            cardsToDiscard = cardsToDiscard.Where(card =>
                card.Owner.Creature.Powers.OfType<BalatroPower>()
                    .All(p => p.ShouldDiscard(card))
            ).ToList();
        }
    }
}