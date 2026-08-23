using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace Balatro.BalatroCode.UI;

public partial class NRerollButton : NButton
{
    public int RerollCost { get; set; } = 50;
    
    protected override string[] Hotkeys => ["r"];
    private Label? _label;
    private TextureRect? _image;

    protected override string? ClickedSfx => "event:/sfx/ui/clicks/ui_click";
    
    public override void _Ready()
    {
        _image = new TextureRect {
            Texture = GD.Load<Texture2D>("res://Balatro/images/ui/reroll_button.png"),
            MouseFilter = MouseFilterEnum.Pass,
            ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            PivotOffset = Size / 2

        };
        _image.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        AddChild(_image);
        
        _label = new MegaLabel
        {
            Text = "Reroll",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            LayoutMode = 1,
            MinFontSize = 24,
            MaxFontSize = 34
        };
        _label.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        _label.AddThemeColorOverride("font_color", new  Color(0.992157f, 0.956863f, 0.890196f));
        _label.AddThemeColorOverride("font_outline_color", new Color(0.121569f, 0.25098f, 0.270588f));
        _label.AddThemeConstantOverride("outline_size", 12);
        _label.AddThemeFontOverride("font", ResourceLoader.Load<Font>("res://themes/kreon_bold_glyph_space_two.tres"));
        _label.AddThemeFontSizeOverride("font_size", 34);
        AddChild(_label);
        
        ConnectSignals();
    }

    protected override void OnFocus()
    {
        _image?.SetScale(new Vector2(1.1f, 1.1f));
        base.OnFocus();
    }

    protected override void OnUnfocus()
    {
        _image?.SetScale(new Vector2(1.0f, 1.0f));
        base.OnUnfocus();
    }
    
}
