using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Balatro.BalatroCode.Patches;

public class DeckRemovalPatches
{
    [HarmonyPatch(typeof(CardSelectCmd), nameof(CardSelectCmd.FromDeckForRemoval))]
    public static class BalatroFromDeckForRemovalPatch
    {
        static void Prefix(Player player, CardSelectorPrefs prefs, ref Func<CardModel, bool>? filter)
        {
            if (Stakes.CurrentStake() < 3) return;
            filter = c => c.Rarity != CardRarity.Basic;
        }
    }
}