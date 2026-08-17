using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Powers;
using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Patches;

[HarmonyPatch(typeof(CardModel), "get_Type")]
public static class PareidoliaTypePatch
{
    public static void Postfix(CardModel __instance, ref CardType __result)
    {
        if (!__instance.IsMutable) return;
        if (__instance.Owner is not { } owner) return;
        if (owner.HasPower<PareidoliaPower>())
        {
            __result = CardType.Power;
        }
    }
}

[HarmonyPatch(typeof(CardModel), "get_CombatState")]
public static class PareidoliaCombatPatch
{
    public static void Postfix(CardModel __instance, ref ICombatState? __result)
    {
        if (!__instance.IsMutable) return;
        if (__instance.Owner is not { } owner) return;
        if (owner.HasPower<PareidoliaPower>() && __result == null)
        {
            __result = owner.Creature.CombatState;;
        }
    }
}