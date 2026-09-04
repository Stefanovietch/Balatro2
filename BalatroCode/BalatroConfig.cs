using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BaseLib.Config;
using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;
using FileAccess = System.IO.FileAccess;

namespace Balatro.BalatroCode;

public static class Stakes
{
    private static readonly Dictionary<string, int> Cache = LoadCache();

    private static Dictionary<string, int> LoadCache()
    {
        try
        {
            var raw = BalatroConfig.Stakes;
            if (string.IsNullOrWhiteSpace(raw))
                return new Dictionary<string, int>();

            return JsonSerializer.Deserialize<Dictionary<string, int>>(raw) 
                   ?? new Dictionary<string, int>();
        }
        catch (JsonException)
        {
            return new Dictionary<string, int>();
        }
    }
    public static int GetMaxStake(string deckName)
    {
        return Cache.GetValueOrDefault(deckName, 0);
    }

    public static void SetMaxStake(string deckName, int value)
    {
        Cache[deckName] = value;
        BalatroConfig.Stakes = JsonSerializer.Serialize(Cache);
        ModConfig.SaveDebounced<BalatroConfig>();
    }

    private static Dictionary<string, int> StakeLevels { get;} = new()
    {
        { "whiteStake", 0 }, { "redStake", 1 }, { "greenStake", 2 }, { "blackStake", 3 }, 
        { "blueStake", 4 }, { "purpleStake", 5 }, { "orangeStake", 6 }, { "goldStake", 7 }
    };

    public static int CurrentStake(Player? player)
    {
        if (RunManager.Instance.NetService.Type != NetGameType.Singleplayer || player?.Character is not Character.Balatro) return -1;
        return StakeLevels.GetValueOrDefault(BalatroConfig.SelectedStake, -1);
    }
}

[ConfigHoverTipsByDefault]
internal class BalatroConfig : SimpleModConfig
{
    [ConfigHoverTip] public static bool BlindsActive { get; set; } = true;
    [ConfigHoverTip] public static bool GoldCap { get; set; } = true;
    [ConfigHoverTip] public static bool CardOverlay { get; set; } = true;

    [ConfigHideInUI] public static string SelectedDeck { get; set; } = "redDeck";
    [ConfigHideInUI] public static string SelectedStake { get; set; } = "whiteStake";
    
    [ConfigHideInUI]
    public static string Stakes { get; set; } = JsonSerializer.Serialize(new Dictionary<string, int>
    {
        { "redDeck", 7 }, { "blueDeck", 7 }, { "yellowDeck", 0 },
        { "greenDeck", 0 }, { "blackDeck", 0 }, { "magicDeck", 0 },
        { "nebulaDeck", 0 }, { "ghostDeck", 3 }, { "abandonedDeck", 0 },
        { "checkeredDeck", 0 }, { "zodiacDeck", 0 }, { "paintedDeck", 0 },
        { "anaglyphDeck", 0 }, { "plasmaDeck", 0 }, { "erraticDeck", 0 }
    });
}