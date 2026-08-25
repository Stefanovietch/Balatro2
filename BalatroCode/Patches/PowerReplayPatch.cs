using Balatro.BalatroCode.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Patches;

[HarmonyPatch(typeof(CardModel), "get_CombatState")]
public static class PowerReplayPatch
{
    public static bool IsReplaying;
    
    [HarmonyPostfix]
    public static void Postfix(CardModel __instance, ref ICombatState? __result)
    {
        if (__result != null) return;
        if (!IsReplaying) return;
        if (__instance.Owner is not { } owner) return;
        __result = owner.Creature.CombatState;
    }
}