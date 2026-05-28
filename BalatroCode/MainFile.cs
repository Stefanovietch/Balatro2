using Balatro.BalatroCode.UI;
using BaseLib.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
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
    
}