using Balatro.BalatroCode.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Patches;

public class ArchaicToothPatch
{
    [HarmonyPatch(typeof(ArchaicTooth), nameof(ArchaicTooth.AfterObtained))]
    public static class ArchaicToothAfterObtainedPatch
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
    
    [HarmonyPatch(typeof(ArchaicTooth), "get_ExtraHoverTips")]
    public static class ArchaicToothExtraHoverTipsPatch
    {
        private static readonly IEnumerable<IHoverTip> HoverTips = new List<IHoverTip>
        {
            HoverTipFactory.FromCard(ModelDb.Card<MadJoker>()),
            HoverTipFactory.FromCard(ModelDb.Card<CleverJoker>()),
            HoverTipFactory.FromCard(ModelDb.Card<EvolvedJoker>())
        };
        
        [HarmonyPostfix]
        static void Postfix(ArchaicTooth __instance, ref IEnumerable<IHoverTip> __result)
        {
            if (!__instance.IsMutable) return;
            if (__instance.Owner?.Character is Character.Balatro)
            {
                __result = HoverTips;
            }
        }
    }
    
    [HarmonyPatch(typeof(RelicModel), "get_Description")]
    public static class ArchaicToothDescriptionPatch
    {
        private static readonly LocString Description = new LocString("relics", "BALATRO-ARCHAIC-TOOTH.description");
        
        [HarmonyPostfix]
        static void Postfix(RelicModel __instance, ref LocString __result)
        {
            if (!__instance.IsMutable) return;
            if (__instance is ArchaicTooth && __instance.Owner?.Character is Character.Balatro)
            {
                __result = Description;
            }
        }
    }
    
    [HarmonyPatch(typeof(RelicModel), "get_EventDescription")] 
    public static class ArchaicToothEventDescriptionPatch
    {
        private static readonly LocString EventDescription = new LocString("relics", "BALATRO-ARCHAIC-TOOTH.eventDescription");

        [HarmonyPostfix]
        static void Postfix(RelicModel __instance, ref LocString __result)
        {
            if (!__instance.IsMutable) return;
            if (__instance is ArchaicTooth && __instance.Owner?.Character is Character.Balatro)
            {
                __result = EventDescription;
            }
        }
    }
}