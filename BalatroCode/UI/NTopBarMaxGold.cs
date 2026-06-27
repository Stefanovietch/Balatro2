using Godot;
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
        base._Ready();

        // Recreate what the .tscn had
        var container = new Control();
        container.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        AddChild(container);

        var icon = new TextureRect();
        icon.Texture = ResourceLoader.Load<Texture2D>("res://Balatro/images/charui/text_energy.png");
        icon.CustomMinimumSize = new Vector2(72, 72);
        icon.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        icon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        container.AddChild(icon);

        var label = new Label();
        label.Name = "CombatGold";
        label.Text = "0 / 200";
        label.HorizontalAlignment = HorizontalAlignment.Right;
        label.VerticalAlignment = VerticalAlignment.Bottom;
        label.AddThemeColorOverride("font_color", new Color(1f, 0.851f, 0.180f));
        label.AddThemeColorOverride("font_outline_color", new Color(0.098f, 0.161f, 0.188f));
        label.AddThemeConstantOverride("outline_size", 12);
        label.AddThemeFontSizeOverride("font_size", 34);
        AddChild(label);
    }

    protected override int? GetGoldEarned()
    {
        if (Player?.Character is not Character.Balatro balatro) return null;
        if (Player.PlayerCombatState == null) return 0;
        return balatro.CombatGoldEarned.Get(Player.PlayerCombatState);
    }
    protected override int? GetMaxGold()
    {
        if (Player?.Character is not Character.Balatro balatro) return null;
        return balatro.MaxCombatGold.Get(balatro);
    }
}