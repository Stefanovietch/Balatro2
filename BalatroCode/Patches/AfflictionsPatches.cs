using Balatro.BalatroCode.Afflictions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Patches;

public class AfflictionsPatches
{
    [HarmonyPatch(typeof(AfflictionModel))]
    [HarmonyPatch("get_OverlayPath")]
    public static class AfflictionModelOverlayPathPatch
    {
        [HarmonyPostfix]
        private static void Postfix(AfflictionModel __instance, ref string __result)
        {
            if (__instance is not BalatroAfflictions balatroAfflictions) return;
            if (string.IsNullOrWhiteSpace(balatroAfflictions.CustomOverlayPath)) return;
            __result = balatroAfflictions.CustomOverlayPath;
        }
    }

    [HarmonyPatch(typeof(AfflictionModel))]
    [HarmonyPatch("get_Title")]
    public static class AfflictionModelTitlePatch
    {
        [HarmonyPostfix]
        private static void Postfix(AfflictionModel __instance, ref LocString __result)
        {
            if (__instance is not BalatroAfflictions balatroAfflictions) return;
            if (string.IsNullOrWhiteSpace(balatroAfflictions.CustomLocalizationKey)) return;
            __result = new LocString("afflictions", $"{balatroAfflictions.CustomLocalizationKey}.title");
        }
    }

    [HarmonyPatch(typeof(AfflictionModel))]
    [HarmonyPatch("get_Description")]
    public static class AfflictionModelDescriptionPatch
    {
        [HarmonyPostfix]
        private static void Postfix(AfflictionModel __instance, ref LocString __result)
        {
            if (__instance is not BalatroAfflictions balatroAfflictions) return;
            if (string.IsNullOrWhiteSpace(balatroAfflictions.CustomLocalizationKey)) return;
            __result = new LocString("afflictions", $"{balatroAfflictions.CustomLocalizationKey}.description");
        }
    }

    [HarmonyPatch(typeof(AfflictionModel))]
    [HarmonyPatch("get_ExtraCardText")]
    public static class AfflictionModelExtraCardTextPatch
    {
        [HarmonyPostfix]
        private static void Postfix(AfflictionModel __instance, ref LocString __result)
        {
            if (__instance is not BalatroAfflictions balatroAfflictions) return;
            if (string.IsNullOrWhiteSpace(balatroAfflictions.CustomLocalizationKey)) return;
            __result = new LocString("afflictions", $"{balatroAfflictions.CustomLocalizationKey}.extraCardText");
        }
    }
}