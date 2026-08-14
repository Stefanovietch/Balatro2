using System.Collections;
using Balatro.BalatroCode.Relics;
using Balatro.BalatroCode.UI;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Commands;
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
            try
            {
                if (__instance.Owner == null) return;
                if (!__instance.Owner.Relics.Contains(ModelDb.Relic<HighStakes>())) return;
                IEnumerable<EventOption> options = __instance.AllPossibleOptions.ToList();
                var relic = options.ElementAt(Random.Shared.Next(0, options.Count())).Relic;
                while (__instance.Owner.Relics.Contains(relic) || relic == null)
                    relic = options.ElementAt(Random.Shared.Next(0, options.Count())).Relic;
                await RelicCmd.Obtain(relic, __instance.Owner);
            }
            catch (Exception ex)
            {
                MainFile.Logger.Warn($"_Ready postfix error: {ex.Message}");
            }
        }
    }
}