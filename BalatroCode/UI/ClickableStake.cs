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
        Name = (StringName)(stakeName + "Stake");
        StakeLevel = stakeLevel;
        CustomMinimumSize = new Vector2(64, 64);
        _hoverTip = new HoverTip(new LocString("static_hover_tips", "BALATRO-BALATRO." + Name + ".title"),
            new LocString("static_hover_tips", "BALATRO-BALATRO." + Name + ".description"));
    }

    public override void _Ready()
    {
        _image = new TextureRect();
        _image.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        _image.ExpandMode = TextureRect.ExpandModeEnum.KeepSize;
        _image.CustomMinimumSize = new Vector2(64, 64);
        _image.Texture = GD.Load<Texture2D>("res://Balatro/images/ui/stakes/" + Name + ".png");
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
        if (Unlocked)
        {
            var tween = CreateTween();
            tween.TweenProperty(this, "scale", new Vector2(1.2f, 1.2f), 0.1);
        }

        var hoverTip = NHoverTipSet.CreateAndShow(this, _hoverTip);
        if (hoverTip != null) hoverTip.GlobalPosition = GlobalPosition + new Vector2(-32f, -Size.Y - 20f);
    }

    protected override void OnUnfocus()
    {
        if (Unlocked)
        {
            var tween = CreateTween();
            tween.TweenProperty(this, "scale", new Vector2(1f, 1f), 0.1);
        }

        NHoverTipSet.Remove((Control)this);
    }

    protected override void OnPress()
    {
        if (!Selected && Unlocked) StakePanelUI.SelectStake(this);
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

    public void LoadStakeState()
    {
        Unlocked = Stakes.GetMaxStake(BalatroConfig.SelectedDeck) >= StakeLevel;
        if (_image == null) return;
        _image.SelfModulate = !Unlocked ? new Color(0.5f, 0.5f, 0.5f, 1f) : new Color(1f, 1f, 1f, 1f);
    }
}