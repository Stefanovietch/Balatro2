using Balatro.BalatroCode.Relics;
using Balatro.BalatroCode.UI;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Saves;

namespace Balatro.BalatroCode.Patches;

public class PopulateStartingRelicsPatches
{
    [HarmonyPatch(typeof(Player), "PopulateStartingRelics")]
    public static class PopulateStartingRelicsPatch
    {
        public static bool Prefix(Player __instance)
        {
            if (__instance.Character is not Character.Balatro balatro) return true;

            try
            {
                var relic = MainFile.GetRelic(BalatroConfig.SelectedDeck);

                var mutable = relic.ToMutable();
                mutable.FloorAddedToDeck = 1;
                SaveManager.Instance.MarkRelicAsSeen(mutable);

                var method = AccessTools.Method(typeof(Player), "PopulateRelics");
                method?.Invoke(__instance, new object[] { new List<RelicModel> { mutable }, false });
                MainFile.Logger.Info($"Starting relic: {relic.Id.Entry} added for {BalatroConfig.SelectedDeck}");
                
                return false;
            }
            catch (Exception ex)
            {
                MainFile.Logger.Warn($"PopulateStartingRelics postfix error: {ex.Message}");
                return true;
            }
        }
    }
}