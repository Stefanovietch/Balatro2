using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BaseLib.Config;
using Godot;
using FileAccess = System.IO.FileAccess;

namespace Balatro.BalatroCode;

public static class Stakes
{
    public static int GetStake(string deckName)
    {
        return BalatroConfig.Stakes.GetValueOrDefault(deckName, 0);
    }

    public static void SetStake(string deckName, int value)
    {
        BalatroConfig.Stakes[deckName] = value;
    }
}

[ConfigHoverTipsByDefault]
internal class BalatroConfig : SimpleModConfig
{
    // Adds a hover tip for just this property, uses .hover.desc and .hover.title (optional) suffixes
    // in localization. Not necessary with [ConfigHoverTipsByDefault] on the class.
    [ConfigHoverTip] public static bool BlindsActive { get; set; } = true;

    [ConfigHoverTip] public static bool GoldCap { get; set; } = true;

    // the new value is written to disk.
    [ConfigHideInUI] public static string SelectedDeck { get; set; } = "redDeck";

    [ConfigHideInUI] public static string SelectedStake { get; set; } = "whiteStake";

    [ConfigHideInUI]
    [System.ComponentModel.TypeConverter(typeof(DictionaryJsonConverter))]
    public static Dictionary<string, int> Stakes { get; set; } = new()
    {
        { "redDeck", 4 }, { "blueDeck", 2 }, { "yellowDeck", 0 },
        { "greenDeck", 0 }, { "blackDeck", 0 }, { "magicDeck", 0 },
        { "nebulaDeck", 0 }, { "ghostDeck", 3 }, { "abandonedDeck", 0 },
        { "checkeredDeck", 0 }, { "zodiacDeck", 0 }, { "paintedDeck", 0 },
        { "anaglyphDeck", 0 }, { "plasmaDeck", 0 }, { "erraticDeck", 0 }
    };

    private sealed class DictionaryJsonConverter : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context,
            System.Globalization.CultureInfo? culture, object value)
        {
            return value is string s
                ? JsonSerializer.Deserialize<Dictionary<string, int>>(s)
                : base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext? context, Type? destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context,
            System.Globalization.CultureInfo? culture, object? value, Type destinationType)
        {
            return destinationType == typeof(string) && value is Dictionary<string, int> dict
                ? JsonSerializer.Serialize(dict)
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}