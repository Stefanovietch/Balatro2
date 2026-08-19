using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Patches;


[HarmonyPatch(typeof(CharacterModel), "get_Title")]
public class RenameTitlePatches
{
    public static void Postfix(CharacterModel __instance, ref LocString __result)
    {
        if (__instance is not Character.Balatro) return;
        __result = new LocString("static_hover_tips", $"BALATRO-BALATRO.{BalatroConfig.SelectedDeck}.title");
    }
}

[HarmonyPatch(typeof(CharacterModel), "get_TitleObject")]
public class RenameTitleObjectPatches
{
    public static void Postfix(CharacterModel __instance, ref LocString __result)
    {
        if (__instance is not Character.Balatro) return;
        __result = new LocString("static_hover_tips", $"BALATRO-BALATRO.{BalatroConfig.SelectedDeck}.title");
    }
}