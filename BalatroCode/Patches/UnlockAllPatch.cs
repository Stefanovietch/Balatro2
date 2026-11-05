namespace Balatro.BalatroCode.Patches;

using HarmonyLib;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;

public class UnlockAllPatch
{
    [HarmonyPatch(typeof(UnlockConsoleCmd), nameof(UnlockConsoleCmd.Process))]
    static class ProcessPatch
    {
        static bool _registered;

        static void Prefix()
        {
            if (_registered) return;
            _registered = true;

            var actions = AccessTools.Field(typeof(UnlockConsoleCmd), "_unlockActions").GetValue(null) as Dictionary<string, Action<UnlockConsoleCmd, List<string>>>;
            var types = AccessTools.Field(typeof(UnlockConsoleCmd), "_validDiscoveryTypes").GetValue(null) as List<string>;

            if (actions != null)
                actions["stakes"] = (_, _) =>
                {
                    foreach (var deck in MainFile.AllDecks)
                    {
                        Stakes.SetMaxStake(deck, 7);
                    }
                };
            types?.Add("stakes");
        }
    }
}