using Balatro.BalatroCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Balatro.BalatroCode.Patches;

public class SceneBootstrapperPatches
{
    [HarmonyPatch(typeof(Character.Balatro), nameof(Character.Balatro.StartingRelics), MethodType.Getter)]
    public static class BalatroStartingRelicsPatch
    {
        public static bool Prefix(Character.Balatro __instance, ref IEnumerable<RelicModel> __result)
        {
            RelicModel relic = MainFile.GetRelic(BalatroConfig.SelectedDeck);

            __result = new List<RelicModel>
            {
                relic
            };

            return false;
        }
    }
}