using Balatro.BalatroCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Balatro.BalatroCode.Patches;

public class SceneBootstrapperPatches
{
    [HarmonyPatch(typeof(Character.Balatro), nameof(Character.Balatro.StartingRelics), MethodType.Getter)]
    public static class BalatroStartingSceneRelicsPatch
    {
        public static bool Prefix(
            Character.Balatro __instance,
            ref IEnumerable<RelicModel> __result)
        {
            RelicModel relic = __instance.SelectedDeck switch
            {
                "redDeck" => ModelDb.Relic<LowStakes>(),
                "blueDeck" => ModelDb.Relic<HeadsUp>(),
                "yellowDeck" => ModelDb.Relic<NestEgg>(),
                "greenDeck" => ModelDb.Relic<BagOfPreparation>(),
                "blackDeck" => ModelDb.Relic<BigHat>(),
                "magicDeck" => ModelDb.Relic<BurningBlood>(),
                "nebulaDeck" => ModelDb.Relic<BurningBlood>(),
                "ghostDeck" => ModelDb.Relic<BurningBlood>(),
                "abandonedDeck" => ModelDb.Relic<BurningBlood>(),
                "checkeredDeck" => ModelDb.Relic<BurningBlood>(),
                "zodiacDeck" => ModelDb.Relic<BurningBlood>(),
                "paintedDeck" => ModelDb.Relic<BurningBlood>(),
                "anaglyphDeck" => ModelDb.Relic<BurningBlood>(),
                "plasmaDeck" => ModelDb.Relic<BurningBlood>(),
                "erraticDeck" => ModelDb.Relic<BurningBlood>(),
                _ => ModelDb.Relic<Lantern>()
            };

            __result = new List<RelicModel>
            {
                relic
            };

            return false;
        }
    }
    
    [HarmonyPatch(typeof(Character.Balatro), nameof(Character.Balatro.StartingRelics), MethodType.Getter)]
    public static class BalatroStartingRelicsPatch
    {
        public static bool Prefix(
            Character.Balatro __instance,
            ref IEnumerable<RelicModel> __result)
        {
            RelicModel relic = __instance.SelectedDeck switch
            {
                "redDeck" => ModelDb.Relic<LowStakes>(),
                "blueDeck" => ModelDb.Relic<HeadsUp>(),
                "yellowDeck" => ModelDb.Relic<NestEgg>(),
                "greenDeck" => ModelDb.Relic<BagOfPreparation>(),
                "blackDeck" => ModelDb.Relic<BigHat>(),
                "magicDeck" => ModelDb.Relic<BurningBlood>(),
                "nebulaDeck" => ModelDb.Relic<BurningBlood>(),
                "ghostDeck" => ModelDb.Relic<BurningBlood>(),
                "abandonedDeck" => ModelDb.Relic<BurningBlood>(),
                "checkeredDeck" => ModelDb.Relic<BurningBlood>(),
                "zodiacDeck" => ModelDb.Relic<BurningBlood>(),
                "paintedDeck" => ModelDb.Relic<BurningBlood>(),
                "anaglyphDeck" => ModelDb.Relic<BurningBlood>(),
                "plasmaDeck" => ModelDb.Relic<BurningBlood>(),
                "erraticDeck" => ModelDb.Relic<BurningBlood>(),
                _ => ModelDb.Relic<Lantern>()
            };

            __result = new List<RelicModel>
            {
                relic
            };

            return false;
        }
    }
}