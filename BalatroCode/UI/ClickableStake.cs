using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.sts2.Core.Nodes.TopBar;

namespace Balatro.BalatroCode.UI;

public partial class ClickableStake : NButton
{
    private TextureRect? _image;
    private readonly HoverTip _hoverTip;
    public bool Selected;
    public bool Unlocked;
    public int StakeLevel;
    private Control? _containingPanel;
    private TextureRect? _selectedTexture;
    
    public ClickableStake(string stakeName, int stakeLevel)
    {
        this.Name = (StringName) (stakeName + "Stake");
        this.StakeLevel = stakeLevel;
        this.CustomMinimumSize = new Vector2(64, 64);
        this._hoverTip = new HoverTip(new LocString("static_hover_tips", "BALATRO-BALATRO."+this.Name+".title"), new LocString("static_hover_tips", "BALATRO-BALATRO."+this.Name+".description"));
    }

    public override void _Ready()
    {
        _image = new TextureRect();
        _image.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        _image.ExpandMode = TextureRect.ExpandModeEnum.KeepSize;
        _image.CustomMinimumSize = new Vector2(64, 64f);
        _image.Texture = GD.Load<Texture2D>("res://Balatro/images/ui/stakes/"+this.Name+".png");
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
        if (this.Unlocked)
        {
            Tween tween = CreateTween();
            tween.TweenProperty(this, "scale", new Vector2(1.2f, 1.2f), 0.1);
        }

        NHoverTipSet.CreateAndShow((Godot.Control) this, (IHoverTip) _hoverTip).GlobalPosition = this.GlobalPosition + new Vector2(-32f, - this.Size.Y - 20f);
    }

    protected override void OnUnfocus()
    {
        if (this.Unlocked)
        {
            Tween tween = CreateTween();
            tween.TweenProperty(this, "scale", new Vector2(1f, 1f), 0.1);
        }

        NHoverTipSet.Remove((Godot.Control) this);
    }
    
    protected override void OnPress()
    {
        if (!Selected && Unlocked) {
            StakePanelUI.SelectStake(this);
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

    public void LoadStakeState()
    {
        this.Unlocked = Stakes.GetStake(BalatroConfig.SelectedDeck) >= this.StakeLevel;
        if (this._image == null) return;
        this._image.SelfModulate = !this.Unlocked ? new Color(0.5f, 0.5f, 0.5f, 1f) : new Color(1f, 1f, 1f, 1f);
    }
}