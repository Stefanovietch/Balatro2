using Balatro.BalatroCode.Relics;
using Balatro.BalatroCode.UI;
using BaseLib.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Runs;

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

        ModHelper.SubscribeForRunStateHooks(
            ModId,
            GetRunStateHooks
        );

        ModHelper.SubscribeForCombatStateHooks(
            ModId,
            GetCombatStateHooks
        );
    }
    
    private static IRunState? _activeCombatRunState;
    private static IEnumerable<AbstractModel> GetRunStateHooks(RunState runState)
    {
        if (ReferenceEquals(runState, _activeCombatRunState))
            yield break;
        
        foreach (var player in runState.Players)
            if (player.Character is Character.Balatro balatro)
                yield return balatro;
    }

    private static IEnumerable<AbstractModel> GetCombatStateHooks(
        CombatState combatState)
    {
        if (!combatState.IsLiveCombat())
        {
            if (ReferenceEquals(_activeCombatRunState, combatState.RunState))
                _activeCombatRunState = null;
            yield break;
        }
        
        _activeCombatRunState = combatState.RunState;
        
        foreach (var player in combatState.Players)
            if (player.Character is Character.Balatro balatro)
                yield return balatro;
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