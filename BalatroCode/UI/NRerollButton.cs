using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace Balatro.BalatroCode.UI;

public partial class NRerollButton : NButton
{

    private MerchantInventory? _inventory;
    public int RerollCost { get; set; } = 100;

    protected override string[] Hotkeys => ["r"];
    protected override string? ClickedSfx => "event:/sfx/ui/clicks/ui_click";

    public void Initialize(MerchantInventory inventory)
    {
        _inventory = inventory;
        UpdateLabel();
    }
    protected override void ConnectSignals()
    {
        base.ConnectSignals();
    }

    protected override async void OnRelease()
    {
        try
        {
            if (_inventory == null) return;

            var player = _inventory.Player;
            if (player.Gold < RerollCost) return;
        
            await PlayerCmd.LoseGold(RerollCost, player, GoldLossType.Spent);

            // Repopulate each card entry
            foreach (var entry in _inventory.CharacterCardEntries)
                entry.Populate();
            foreach (var entry in _inventory.ColorlessCardEntries)
                entry.Populate();

            UpdateLabel();
        }
        catch (Exception ex)
        {
            MainFile.Logger.Warn($"NRerollButton OnRelease error: {ex.Message}");
        }
    }

    private void UpdateLabel()
    {
        // If you add a Label child node, update it here
        // e.g.: GetNode<Label>("%Label").Text = $"Reroll ({RerollCost}g)";
    }
}