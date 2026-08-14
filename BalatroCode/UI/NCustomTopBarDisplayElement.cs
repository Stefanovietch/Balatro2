using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace Balatro.BalatroCode.UI;

public abstract partial class NCustomTopBarDisplayElement : NClickableControl, ITopBarElement
{
    private static NCustomTopBarDisplayElement? _instance;
    private Tween? _bumpTween;
    private MegaLabel? _countLabel;
    private float _elapsedTime;

    private Control? _icon;
    private float _previousCount;
    protected Player? Player;


    // ── Lifecycle ─────────────────────────────────────────────────────────────

    /// <summary>Node path to the icon Control that wobbles on hover.</summary>
    protected abstract string IconNodePath { get; }

    /// <summary>Node path to the MegaLabel showing the count badge.</summary>
    protected abstract string CountLabelNodePath { get; }

    // ── ITopBarElement ────────────────────────────────────────────────────────

    public abstract string ScenePath { get; }
    public abstract float Width { get; }
    public abstract Func<Player, bool> CanUse { get; }

    public void Initialize(Player player)
    {
        Player = player;
        _instance = this;
        RefreshCount();
    }

    public override void _Ready()
    {
        ConnectSignals();
        _icon = GetNodeOrNull<Control>(IconNodePath);
        _countLabel = GetNodeOrNull<MegaLabel>(CountLabelNodePath);
    }

    // ── Count badge ───────────────────────────────────────────────────────────

    /// <summary>Returns the value to show on the badge, or null to hide it.</summary>
    protected abstract int? GetGoldEarned();

    protected abstract int? GetMaxGold();

    public void RefreshCount()
    {
        if (_countLabel == null) return;
        var goldEarned = GetGoldEarned();
        var maxGold = GetMaxGold();

        if (goldEarned == null || maxGold == null)
        {
            _countLabel.Visible = false;
            return;
        }

        _countLabel.Visible = true;

        if (goldEarned > _previousCount)
        {
            _bumpTween?.Kill();
            _bumpTween = CreateTween();
            _bumpTween.TweenProperty(_countLabel, "scale", Vector2.One, 0.5f)
                .From(Vector2.One * 1.5f)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Expo);
            _countLabel.PivotOffset = _countLabel.Size * 0.5f;
        }

        _previousCount = goldEarned.Value;
        _countLabel.SetTextAutoSize(goldEarned.Value.ToString() + " / " + maxGold.Value.ToString());
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

    public static void RefreshDisplay()
    {
        _instance?.RefreshCount();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_instance == this) _instance = null;
    }
}