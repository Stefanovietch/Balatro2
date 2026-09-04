using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Character;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Patches;

public class UnlockStatePatches
{
    public static class PlayerContext
    {
        public static Player? Current;
    }
    
    [HarmonyPatch(typeof(Player), nameof(Player.UnlockState), MethodType.Getter)]
    private static class CaptureCurrentPlayer
    {
        [HarmonyPrefix]
        static void Prefix(Player __instance) => PlayerContext.Current = __instance;
    }
    
    [HarmonyPatch(typeof(CardPoolModel), nameof(CardPoolModel.GetUnlockedCards))]
    private static class FilterGrosMichel
    {
        [HarmonyPostfix]
        static void Postfix(CardPoolModel __instance, ref IEnumerable<CardModel> __result)
        {
            if (__instance is not BalatroCardPool) return;
            var player = PlayerContext.Current;
            if (player == null) return;
            bool isExtinct = Character.Balatro.GrosMichelExtinct.Get(player);
            __result = __result.Where(c => isExtinct ? c is not GrosMichel : c is not Cavendish);
        }
    }
}