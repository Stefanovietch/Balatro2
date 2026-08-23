using System.Diagnostics;
using Balatro.BalatroCode.UI;
using BaseLib.Config;
using Godot;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

namespace Balatro.BalatroCode.UI;

public static class StakePanelUI
{
    private static Control? _stakePanel;
    private static NCharacterSelectScreen? _screen;

    public static void Attach(NCharacterSelectScreen screen)
    {
        Callable.From(() => DoAttach(screen)).CallDeferred();
    }

    private static void DoAttach(NCharacterSelectScreen screen)
    {
        try
        {
            if (_stakePanel != null && GodotObject.IsInstanceValid(_stakePanel) && _stakePanel.IsInsideTree())
            {
                MainFile.Logger.Info("Overlay already attached");
                return;
            }
            _screen = screen;
            _stakePanel = new GridContainer
            {
                CustomMinimumSize = new Vector2(420, 56),
                Columns = 8
            };
            _stakePanel.AddThemeConstantOverride("h_separation", 30);
            _stakePanel.AddThemeConstantOverride("v_separation", 30);

            PositionHbox(_stakePanel);

            _stakePanel.AddChild(new ClickableStake("white", 0));
            _stakePanel.AddChild(new ClickableStake("red", 1));
            _stakePanel.AddChild(new ClickableStake("green", 2));
            _stakePanel.AddChild(new ClickableStake("black", 3));
            _stakePanel.AddChild(new ClickableStake("blue", 4));
            _stakePanel.AddChild(new ClickableStake("purple", 5));
            _stakePanel.AddChild(new ClickableStake("orange", 6));
            _stakePanel.AddChild(new ClickableStake("gold", 7));

            LayoutDecks();

            screen.AddChild(_stakePanel);

            LoadStakes(BalatroConfig.SelectedStake);
        }
        catch (Exception ex)
        {
            MainFile.Logger.Warn($"overlay attach failed: {ex.Message}");
        }
    }

    public static void PositionHbox(Control c, int width = 8 * 84, int height = 1 * 84, int offsetRight = 300,
        int offsetTop = 550)
    {
        c.AnchorLeft = 1f;
        c.AnchorRight = 1f;
        c.AnchorTop = 0f;
        c.AnchorBottom = 0f;
        c.OffsetLeft = -(offsetRight + width);
        c.OffsetRight = -offsetRight;
        c.OffsetTop = offsetTop;
        c.OffsetBottom = offsetTop + height;
        c.GrowHorizontal = Control.GrowDirection.Begin;
        c.GrowVertical = Control.GrowDirection.End;
    }

    private static void LayoutDecks()
    {
        if (_stakePanel == null)
        {
            MainFile.Logger.Warn($"No deckPanel");
            return;
        }

        foreach (var node in _stakePanel.FindChildren("*", owned: false))
        {
            if (node is not ClickableStake stake) continue;
            stake.SetPanel(_stakePanel);
        }
    }

    public static void SelectStake(ClickableStake selected)
    {
        if (_stakePanel == null)
        {
            MainFile.Logger.Warn($"{selected} not in stakePanel");
            return;
        }

        foreach (var node in _stakePanel.FindChildren("*", owned: false))
        {
            if (node is not ClickableStake stake) continue;
            if (stake != selected)
            {
                stake.Selected = false;
                stake.OnDeselect();
            }
            else
            {
                stake.Selected = true;
                stake.OnSelect();
                BalatroConfig.SelectedStake = stake.Name;
                ModConfig.SaveDebounced<BalatroConfig>();
            }
        }
    }

    public static void LoadStakes(StringName selectedStake)
    {
        if (_stakePanel == null)
        {
            MainFile.Logger.Warn($"could not load stakePanel");
            return;
        }

        foreach (var node in _stakePanel.FindChildren("*", owned: false))
        {
            if (node is not ClickableStake stake) continue;
            stake.LoadStakeState();

            if (stake.Name != selectedStake) continue;
            if (stake.Unlocked)
            {
                stake.Selected = true;
                stake.OnSelect();
            }
            else if (selectedStake != "whiteStake")
            {
                stake.Selected = false;
                stake.OnDeselect();
                BalatroConfig.SelectedStake = "whiteStake";
                ModConfig.SaveDebounced<BalatroConfig>();
                LoadStakes("whiteStake");
            }
            else
            {
                MainFile.Logger.Warn($"unable to load stake: {selectedStake}");
            }
        }
    }

    public static void SetVisibility(bool visible)
    {
        if (_stakePanel == null) return;
        _stakePanel.Visible = _screen?.Lobby.NetService.Type == NetGameType.Singleplayer && visible;
    }
}