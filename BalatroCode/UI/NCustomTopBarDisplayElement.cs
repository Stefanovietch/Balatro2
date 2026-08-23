using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace Balatro.BalatroCode.UI;

public abstract partial class NCustomTopBarDisplayElement : NClickableControl, ITopBarElement
{
    private Tween? _bumpTween;
    private MegaLabel? _countLabel;
    private float _elapsedTime;

    private Control? _icon;
    private float _previousCount;
    protected Player? Player;
    
    protected abstract string IconNodePath { get; }
    protected abstract string CountLabelNodePath { get; }
    public abstract string ScenePath { get; }
    public abstract float Width { get; }
    public abstract Func<Player, bool> CanUse { get; }

    public void Initialize(Player player)
    {
        Player = player;
        UpdateGoldDisplay();
    }
    
    public override void _Ready()
    {
        ConnectSignals();
        _icon = GetNodeOrNull<Control>(IconNodePath);
        _countLabel = GetNodeOrNull<MegaLabel>(CountLabelNodePath);
        Character.Balatro.CombatGoldEarnedChanged -= OnCombatGoldChanged;
        Character.Balatro.CombatGoldEarnedChanged += OnCombatGoldChanged;
        
        Character.Balatro.CombatStart -= ResetGoldDisplay;
        Character.Balatro.CombatStart += ResetGoldDisplay;
    }

    // ── Count badge ───────────────────────────────────────────────────────────

    /// <summary>Returns the value to show on the badge, or null to hide it.</summary>
    protected abstract int? GetGoldEarned();

    protected abstract int? GetMaxGold();
    
    private void OnCombatGoldChanged(Player player)
    {
        if (Player != player)
            return;
        UpdateGoldDisplay();
    }

    private void ResetGoldDisplay()
    {
        if (_countLabel == null) return;
        _countLabel.SetText("0 / 200");
    }
    
    private void UpdateGoldDisplay()
    {    
        var goldEarned = GetGoldEarned();
        var maxGold = GetMaxGold();
        if (_countLabel == null) return;
        if (goldEarned == null || maxGold == null) return;
        
        _bumpTween?.Kill();
        _bumpTween = CreateTween();
        _bumpTween.TweenProperty(_countLabel, "scale", Vector2.One, 0.5f)
            .From(Vector2.One * 1.1f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Expo);
        _countLabel.PivotOffset = _countLabel.Size * 0.5f;
        _previousCount = goldEarned.Value;
        _countLabel.SetText(goldEarned.Value + " / " + maxGold.Value);
    }

    public override void _Process(double delta)
    {
        if (!IsFocused || _icon == null) return;
        _elapsedTime += (float)delta * 4f;
        _icon.Rotation = 0.12f * Mathf.Sin(_elapsedTime);
    }

    protected override void OnFocus()
    {
        base.OnFocus();
        _elapsedTime = 0;
    }

    protected override void OnUnfocus()
    {
        base.OnUnfocus();
        if (_icon == null) return;
        _icon.Rotation = 0f;
    }
    
    public override void _ExitTree()
    {
        Character.Balatro.CombatGoldEarnedChanged -= OnCombatGoldChanged;
        Character.Balatro.CombatStart -= ResetGoldDisplay;

        base._ExitTree();
    }
}