using System.Collections;
using Balatro.BalatroCode.Relics;
using Balatro.BalatroCode.UI;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

namespace Balatro.BalatroCode.Patches;

public class AncientEventModelPatches
{
    [HarmonyPatch(typeof(AncientEventModel), "Done")]
    public static class AncientEventModelPatch
    {
        [HarmonyPostfix]
        public static async void Postfix(AncientEventModel __instance)
        {
            if (!__instance.IsMutable || __instance.Owner is not { } player) return;
            if (player.GetRelic<HighStakes>() == null) return;
            var pickedRelic = player.Relics.LastOrDefault();
            List<RelicModel?> options = __instance.AllPossibleOptions.Select(o => o.Relic).Where(r => r?.GetType() != pickedRelic?.GetType()).ToList();
            var relic = player.PlayerRng.Rewards.NextItem(options);
            if (relic == null) return;
            await RelicCmd.Obtain(relic, player);
        }
    }
    
    [HarmonyPatch(typeof(Neow), "get_PositiveOptions")]
    public static class NeowPreciseScissorsPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Neow __instance, ref IEnumerable<EventOption> __result)
        {
            if (!__instance.IsMutable || Stakes.CurrentStake(__instance.Owner) < 3) return;
            __result = __result.Where(e => e.Relic is not PreciseScissors);
        }
    }
    
    [HarmonyPatch(typeof(Neow), "get_CurseOptions")]
    public static class NeowPrecariousShearsPatch 
    {
        [HarmonyPostfix]
        public static void Postfix(Neow __instance, ref IEnumerable<EventOption> __result)
        {
            if (!__instance.IsMutable || Stakes.CurrentStake(__instance.Owner) < 3) return;
            __result = __result.Where(e => e.Relic is not PrecariousShears);
        }
    }
    
    [HarmonyPatch(typeof(Orobas), "get_OptionPool3")]
    public static class TouchOfOrobasPatch 
    {
        [HarmonyPostfix]
        public static void Postfix(Neow __instance, ref IEnumerable<EventOption> __result)
        {
            if (!__instance.IsMutable) return;
            __result = __result.Where(e => e.Relic is not TouchOfOrobas);
        }
    }
}