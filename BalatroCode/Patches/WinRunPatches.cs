using Balatro.BalatroCode.Afflictions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Patches;

public class WinRunPatches
{
    [HarmonyPatch(typeof(RunManager), "WinRun")]
    public static class WinRunPatch
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            var runState = Traverse.Create(RunManager.Instance).Property("State").GetValue<RunState>();
            if (runState == null) return;
            var player = LocalContext.GetMe(runState);
            var stake = Stakes.CurrentStake(player);
            Stakes.SetMaxStake(BalatroConfig.SelectedDeck, stake + 1);
        }
    }
}