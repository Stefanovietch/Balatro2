using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;

namespace Balatro.BalatroCode.UI;


[GlobalClass]
public partial class NTopBarMaxGold : NCustomTopBarDisplayElement
{
    public override string ScenePath => ""; // unused now
    public override float Width => 200f;
    protected override string IconNodePath => "Control";
    protected override string CountLabelNodePath => "CombatGold";

    public override Func<Player, bool> CanUse =>
        player => player.Character == ModelDb.Character<Character.Balatro>();

    public override void _Ready()
    {

        // Recreate what the .tscn had
        var container = new Control();
        container.Position = new Vector2(0, 0);
        container.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        AddChild(container);

        var icon = new TextureRect();
        icon.Texture = ResourceLoader.Load<Texture2D>("res://Balatro/images/charui/gold_per_combat.png");
        icon.CustomMinimumSize = new Vector2(54, 54);
        icon.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        icon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        icon.Position = new Vector2(0, 15);
        container.AddChild(icon);

        var label = new MegaLabel();
        label.Name = "CombatGold";
        label.Text = "0 / 200";
        label.Position = new Vector2(60, 20);
        label.HorizontalAlignment = HorizontalAlignment.Right;
        label.VerticalAlignment = VerticalAlignment.Center;
        label.AutoSizeEnabled = false; 
        label.AddThemeColorOverride("font_color", new Color(0.937255f, 0.784314f, 0.317647f));
        label.AddThemeColorOverride("font_outline_color", new Color(0.0980392f, 0.160784f, 0.188235f));
        label.AddThemeConstantOverride("outline_size", 12);
        label.AddThemeFontOverride("font", ResourceLoader.Load<Font>("res://themes/kreon_bold_glyph_space_two.tres"));
        label.AddThemeFontSizeOverride("font_size", 32);
        AddChild(label);
        
        base._Ready();
    }

    protected override int? GetGoldEarned()
    {
        if (Player?.Character is not Character.Balatro balatro) return null;
        if (Player.PlayerCombatState == null) return 0;
        return Character.Balatro.CombatGoldEarned.Get(Player.PlayerCombatState);
    }
    protected override int? GetMaxGold()
    {
        if (Player?.Character is not Character.Balatro balatro) return null;
        return Character.Balatro.MaxCombatGold.Get(Player);
    }
}