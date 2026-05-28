using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BaseLib.Config;
using Godot;
using FileAccess = System.IO.FileAccess;

namespace Balatro.BalatroCode;

public static class Stakes
{
    
    public static int GetStake(string deckName) =>
        BalatroConfig.Stakes.GetValueOrDefault(deckName, 0);

    public static void SetStake(string deckName, int value) =>
        BalatroConfig.Stakes[deckName] = value;
    
}

[ConfigHoverTipsByDefault]
internal class BalatroConfig : SimpleModConfig {
    // Adds a hover tip for just this property, uses .hover.desc and .hover.title (optional) suffixes
    // in localization. Not necessary with [ConfigHoverTipsByDefault] on the class.
    [ConfigHoverTip]
    public static bool BlindsActive { get; set; } = true;
    
    [ConfigHoverTip]
    public static bool GoldCap { get; set; } = true;
    
    // the new value is written to disk.
    [ConfigHideInUI]
    public static StringName SelectedDeck { get; set; } = "redDeck";
    
    [ConfigHideInUI]
    public static StringName SelectedStake { get; set; } = "whiteStake";

    [ConfigHideInUI] 
    public static Dictionary<string, int> Stakes { get; set; } = new()
    {
        { "redDeck", 4 }, { "blueDeck", 2 }, { "yellowDeck", 0 },
        { "greenDeck", 0 }, { "blackDeck", 0 }, { "magicDeck", 0 },
        { "nebulaDeck", 0 }, { "ghostDeck", 3 }, { "abandonedDeck", 0 },
        { "checkeredDeck", 0 }, { "zodiacDeck", 0 }, { "paintedDeck", 0 },
        { "anaglyphDeck", 0 }, { "plasmaDeck", 0 }, { "erraticDeck", 0 }
    };

}