using System.Diagnostics;
using Balatro.BalatroCode.Relics;
using Balatro.BalatroCode.UI;
using BaseLib.Config;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Balatro.BalatroCode.UI;

public static class DeckPanelUI
{
    private static Control? _deckPanel;
    private static TextureRect? _relicIcon;
    private static MegaRichTextLabel? _relicTitle;
    private static MegaRichTextLabel? _relicDescription;

    public static void Attach(Node screen)
    {
        Callable.From(() => DoAttach(screen)).CallDeferred();
    }

    private static void DoAttach(Node screen)
    { 
        try
        {
            if (_deckPanel != null)
            {
                MainFile.Logger.Info("overlay already attached and in tree; skipping re-attach");
                return;
            }

            _deckPanel = new GridContainer
            {
                CustomMinimumSize = new Vector2(420, 56),
                Columns = 8,
            };
            _deckPanel.AddThemeConstantOverride("h_separation", 20);
            _deckPanel.AddThemeConstantOverride("v_separation", 20);
            
            PositionHbox(_deckPanel);
            _deckPanel.AddChild(new ClickableDeck("red"));
            _deckPanel.AddChild(new ClickableDeck("blue"));
            _deckPanel.AddChild(new ClickableDeck("yellow"));
            _deckPanel.AddChild(new ClickableDeck("green"));
            
            _deckPanel.AddChild(new ClickableDeck("black"));
            _deckPanel.AddChild(new ClickableDeck("magic"));
            _deckPanel.AddChild(new ClickableDeck("nebula"));
            _deckPanel.AddChild(new ClickableDeck("ghost"));
            _deckPanel.AddChild(new ClickableDeck("abandoned"));
            _deckPanel.AddChild(new ClickableDeck("checkered"));
            _deckPanel.AddChild(new ClickableDeck("zodiac"));
            _deckPanel.AddChild(new ClickableDeck("painted"));
            _deckPanel.AddChild(new ClickableDeck("anaglyph"));
            _deckPanel.AddChild(new ClickableDeck("plasma"));
            _deckPanel.AddChild(new ClickableDeck("erratic"));
            
            LayoutDecks();
            
            screen.AddChild(_deckPanel);
            
            _relicIcon = screen.GetNode<TextureRect>("InfoPanel/VBoxContainer/Relic/Icon");
            _relicTitle = screen.GetNode<MegaRichTextLabel>((NodePath) "InfoPanel/VBoxContainer/Relic/Name/RichTextLabel");
            _relicDescription = screen.GetNode<MegaRichTextLabel>((NodePath) "InfoPanel/VBoxContainer/Relic/Description");
            
            LoadSelectDeck(BalatroConfig.SelectedDeck);
            UpdateRelic(BalatroConfig.SelectedDeck);
        }
        catch (Exception ex)
        {
            MainFile.Logger.Warn($"overlay attach failed: {ex.Message}");
        }
    }
    
    private static void PositionHbox(Control c, int width = 8*84, int  height = 2*84, int offsetRight = 300, int offsetTop = 350)
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
        if (_deckPanel == null)
        {
            MainFile.Logger.Warn($"No deckPanel");
            return;
        }
        foreach (var node in _deckPanel.FindChildren("*", owned: false))
        {
            if (node is not ClickableDeck deck) continue;
            deck.SetPanel(_deckPanel);
        }
    }
    
    public static void SelectDeck(ClickableDeck selected)
    {
        if (_deckPanel == null)
        {
            MainFile.Logger.Warn($"{selected} not in deckPanel");
            return;
        }
        foreach (var node in _deckPanel.FindChildren("*", owned: false))
        {
            if (node is not ClickableDeck deck) continue;
            if (deck != selected)
            {
                deck.Selected = false;
                deck.OnDeselect();
            }
            else
            {
                deck.Selected = true;
                deck.OnSelect();
                BalatroConfig.SelectedDeck = deck.Name;
                ModConfig.SaveDebounced<BalatroConfig>();
                StakePanelUI.LoadStakes(BalatroConfig.SelectedStake);
            }
        }

        UpdateRelic(selected.Name);
    }
    
    private static void LoadSelectDeck(StringName selected)
    {
        if (_deckPanel == null)
        {
            MainFile.Logger.Warn($"Could not load selected deck");
            return;
        }
        foreach (var node in _deckPanel.FindChildren("*", owned: false))
        {
            if (node is not ClickableDeck deck) continue;
            if (deck.Name != selected) continue;
            deck.Selected = true;
            deck.OnSelect();
        }
    }

    public static void SetVisibility(bool visible)
    {
        if (_deckPanel == null) return;
        _deckPanel.Visible = visible;
    }

    private static void UpdateRelic(string deckName)
    {
        RelicModel relic = deckName switch
        {
            "redDeck" => ModelDb.Relic<LowStakes>(),
            "blueDeck" => ModelDb.Relic<HeadsUp>(),
            "yellowDeck" => ModelDb.Relic<NestEgg>(),
            "greenDeck" => ModelDb.Relic<BagOfPreparation>(),
            "blackDeck" => ModelDb.Relic<BigHat>(),
            "magicDeck" => ModelDb.Relic<BurningBlood>(),
            "nebulaDeck" => ModelDb.Relic<BurningBlood>(),
            "ghostDeck" => ModelDb.Relic<BurningBlood>(),
            "abandonedDeck" => ModelDb.Relic<BurningBlood>(),
            "checkeredDeck" => ModelDb.Relic<BurningBlood>(),
            "zodiacDeck" => ModelDb.Relic<BurningBlood>(),
            "paintedDeck" => ModelDb.Relic<BurningBlood>(),
            "anaglyphDeck" => ModelDb.Relic<BurningBlood>(),
            "plasmaDeck" => ModelDb.Relic<BurningBlood>(),
            "erraticDeck" => ModelDb.Relic<BurningBlood>(),
            _ => ModelDb.Relic<Lantern>()
        };

        if (_relicIcon == null || _relicTitle == null || _relicDescription == null) return;
        _relicIcon.Texture = relic.Icon;
        _relicTitle.Text = relic.Title.GetFormattedText();
        _relicDescription.Text = relic.DynamicDescription.GetFormattedText();
    }
}