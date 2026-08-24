using Balatro.BalatroCode.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Patches;

public class ArchaicToothPatch
{
    [HarmonyPatch(typeof(ArchaicTooth), nameof(ArchaicTooth.AfterObtained))]
    public static class BalatroAfterRoomEnteredPatch
    {        
        [HarmonyPostfix]
        static void Postfix(ArchaicTooth __instance)
        {
            var cards = PileType.Deck.GetPile(__instance.Owner).Cards
                .Where(c => c is CleverJoker or MadJoker).ToList();
            if (cards.Count == 0) return;
            foreach (var card in cards) card.RemoveFromCurrentPile();
        }
    }
    
    [HarmonyPatch(typeof(RelicModel), "get_Description")]
    public static class ArchaicToothDescriptionPatch
    {
        [HarmonyPostfix]
        static void Postfix(RelicModel __instance, ref LocString __result)
        {
            if (__instance is ArchaicTooth && __instance.IsMutable && __instance.Owner.Character is Character.Balatro)
            {
                __result = new LocString("relics", "BALATRO-ARCHAIC-TOOTH.description");
            }
        }
    }
    
    [HarmonyPatch(typeof(RelicModel), "get_EventDescription")] 
    public static class ArchaicToothEventDescriptionPatch
    {
        [HarmonyPostfix]
        static void Postfix(RelicModel __instance, ref LocString __result)
        {
            if (__instance is ArchaicTooth && __instance.IsMutable && __instance.Owner.Character is Character.Balatro)
            {
                __result = new LocString("relics", "BALATRO-ARCHAIC-TOOTH.eventDescription");
            }
        }
    }
}