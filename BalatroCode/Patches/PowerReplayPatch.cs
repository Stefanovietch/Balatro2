using Balatro.BalatroCode.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Patches;

[HarmonyPatch(typeof(CardModel), "get_CombatState")]
public static class PowerReplayPatch
{
    public static void Postfix(CardModel __instance, ref ICombatState? __result)
    {
        if (!__instance.IsMutable || !__instance.IsInCombat) return;
        if (__instance.Owner is not { } owner) return;
        if (PileType.Play.GetPile(owner).Cards.LastOrDefault() is (Blueprint or Brainstorm) && __result == null)
        {
            __result = owner.Creature.CombatState;;
        }
    }
}