using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.sts2.Core.Nodes.TopBar;

namespace Balatro.BalatroCode.UI;

public partial class ClickableDeck : NButton
{
    private TextureRect? _image;
    private readonly HoverTip _hoverTip;
    public bool Selected;
    private Control? _containingPanel;
    private TextureRect? _selectedTexture;

    public ClickableDeck(string deckName)
    {
        Name = (StringName)(deckName + "Deck");
        CustomMinimumSize = new Vector2(64, 64);
        _hoverTip = new HoverTip(new LocString("static_hover_tips", "BALATRO-BALATRO." + Name + ".title"),
            new LocString("static_hover_tips", "BALATRO-BALATRO." + Name + ".description"));
    }

    public override void _Ready()
    {
        _image = new TextureRect();
        _image.Name = (StringName)"Image";
        _image.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        _image.ExpandMode = TextureRect.ExpandModeEnum.KeepSize;
        _image.CustomMinimumSize = new Vector2(32, 64f);
        _image.Texture = GD.Load<Texture2D>("res://Balatro/images/decks/small/" + Name + ".png");
        _image.SetAnchorsPreset(LayoutPreset.Center);
        AddChild(_image);

        _selectedTexture = new TextureRect();
        _selectedTexture.Texture = GD.Load<Texture2D>("res://Balatro/images/ui/tiny_star.png");
        _selectedTexture.CustomMinimumSize = new Vector2(40, 40);
        _selectedTexture.SetAnchorsPreset(LayoutPreset.TopLeft);
        _selectedTexture.Position = new Vector2(20, 20);
        _selectedTexture.Visible = false;
        _selectedTexture.MouseFilter = MouseFilterEnum.Ignore;
        AddChild(_selectedTexture);

        Selected = false;
        PivotOffset = Size;

        ConnectSignals();
    }

    protected override void OnFocus()

    {
        var tween = CreateTween();
        tween.TweenProperty(this, "scale", new Vector2(1.2f, 1.2f), 0.1);

        var hoverTip = NHoverTipSet.CreateAndShow(this, _hoverTip);
        if (hoverTip != null) hoverTip.GlobalPosition = GlobalPosition + new Vector2(-32f, -Size.Y - 20f);
    }

    protected override void OnUnfocus()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "scale", new Vector2(1f, 1f), 0.1);

        NHoverTipSet.Remove((Control)this);
    }

    protected override void OnPress()
    {
        if (!Selected) DeckPanelUI.SelectDeck(this);
    }

    public void OnSelect()
    {
        if (_selectedTexture != null) _selectedTexture.Visible = true;
    }

    public void OnDeselect()
    {
        if (_selectedTexture != null) _selectedTexture.Visible = false;
    }

    public void SetPanel(Control panel)
    {
        _containingPanel = panel;
    }
}