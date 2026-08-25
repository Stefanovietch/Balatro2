using Balatro.BalatroCode.Enchantments;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Patches;

public class UpgradePatches
{
    [HarmonyPatch(typeof(CardModel), "get_IsUpgradable")]
    public static class BlueSealIsUpgradablePatch
    {
        static void Postfix(CardModel __instance, ref bool __result)
        {
            if (__instance.Enchantment is BlueSeal)
            {
                __result = true;
            }
        }
    }
    
    [HarmonyPatch(typeof(CardModel), "get_MaxUpgradeLevel")]
    public static class BlueSealMaxUpgradePatch
    {
        static void Postfix(CardModel __instance, ref int __result)
        {
            if (__instance.Enchantment is BlueSeal)
            {
                __result = 9999;
            }
        }
    }
}