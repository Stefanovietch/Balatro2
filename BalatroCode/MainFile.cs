using Balatro.BalatroCode.Relics;
using Balatro.BalatroCode.UI;
using BaseLib.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;

namespace Balatro.BalatroCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "Balatro"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        ModConfigRegistry.Register(ModId, new BalatroConfig());
        Harmony harmony = new(ModId);

        harmony.PatchAll();
        Run();
    }

    private static void Run()
    {

    }

    public void BeginRun()
    {
        
    }

    public static RelicModel GetRelic(string deckName)
    {
        RelicModel relic = deckName switch
        {
            "redDeck" => ModelDb.Relic<LowStakes>(),
            "blueDeck" => ModelDb.Relic<HeadsUp>(),
            "yellowDeck" => ModelDb.Relic<NestEgg>(),
            "greenDeck" => ModelDb.Relic<YouGetWhatYouGet>(),
            "blackDeck" => ModelDb.Relic<Royale>(),
            "magicDeck" => ModelDb.Relic<CrystalBall>(),
            "nebulaDeck" => ModelDb.Relic<Astronomy>(),
            "ghostDeck" => ModelDb.Relic<Clairvoyance>(),
            "abandonedDeck" => ModelDb.Relic<Flushed>(),
            "checkeredDeck" => ModelDb.Relic<Retrograde>(),
            "zodiacDeck" => ModelDb.Relic<ROI>(),
            "paintedDeck" => ModelDb.Relic<BigHands>(),
            "anaglyphDeck" => ModelDb.Relic<HighStakes>(),
            "plasmaDeck" => ModelDb.Relic<RuleBender>(),
            "erraticDeck" => ModelDb.Relic<Shattered>(),
            _ => ModelDb.Relic<BurningBlood>()
        };

        return relic;
    }
}