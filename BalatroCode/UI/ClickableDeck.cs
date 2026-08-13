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
        this.Name = (StringName) (deckName + "Deck");
        this.CustomMinimumSize = new Vector2(64, 64);
        this._hoverTip = new HoverTip(new LocString("static_hover_tips", "BALATRO-BALATRO."+this.Name+".title"), new LocString("static_hover_tips", "BALATRO-BALATRO."+this.Name+".description"));
    }

    public override void _Ready()
    {
        _image = new TextureRect();
        _image.Name = (StringName) "Image";
        _image.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        _image.ExpandMode = TextureRect.ExpandModeEnum.KeepSize;
        _image.CustomMinimumSize = new Vector2(32, 64f);
        _image.Texture = GD.Load<Texture2D>("res://Balatro/images/decks/small/"+this.Name+".png");
        _image.SetAnchorsPreset(Control.LayoutPreset.Center);
        AddChild(_image);
        
        _selectedTexture = new TextureRect();
        _selectedTexture.Texture = GD.Load<Texture2D>("res://Balatro/images/ui/tiny_star.png");
        _selectedTexture.CustomMinimumSize = new Vector2(40, 40);
        _selectedTexture.SetAnchorsPreset(Control.LayoutPreset.TopLeft);
        _selectedTexture.Position = new Vector2(20, 20);
        _selectedTexture.Visible = false;
        _selectedTexture.MouseFilter = Control.MouseFilterEnum.Ignore;
        AddChild(_selectedTexture);
        
        this.Selected = false;
        this.PivotOffset = this.Size;

        this.ConnectSignals();
    }

    protected override void OnFocus()

    {
        Tween tween = CreateTween();
        tween.TweenProperty(this, "scale", new Vector2(1.2f, 1.2f), 0.1);
        
        var hoverTip = NHoverTipSet.CreateAndShow(this, _hoverTip);
        if (hoverTip != null) hoverTip.GlobalPosition = this.GlobalPosition + new Vector2(-32f, - this.Size.Y - 20f);
    }

    protected override void OnUnfocus()
    {
        Tween tween = CreateTween();
        tween.TweenProperty(this, "scale", new Vector2(1f, 1f), 0.1);
        
        NHoverTipSet.Remove((Godot.Control) this);
    }
    
    protected override void OnPress()
    {
        if (!Selected) {
            DeckPanelUI.SelectDeck(this);
        }
    }

    public void OnSelect()
    {
        if (_selectedTexture != null) _selectedTexture.Visible = true;
    }

    public void OnDeselect()
    {
        if (_selectedTexture != null) _selectedTexture.Visible = false;
    }
    
    public void SetPanel(Control panel) {
        this._containingPanel = panel;
    }
}