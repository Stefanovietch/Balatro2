using Godot;
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
    private Label? _label;

    protected override string? ClickedSfx => "event:/sfx/ui/clicks/ui_click";

    public void Initialize(MerchantInventory inventory)
    {
        Name = "RerollButton";
        CustomMinimumSize = new Vector2(160, 48);
        MouseFilter = MouseFilterEnum.Stop;
        FocusMode = FocusModeEnum.All;
        _inventory = inventory;
    }
    
    public override void _Ready()
    {

        _label = new Label
        {
            Text = $"Reroll ({RerollCost}g)",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MouseFilter = MouseFilterEnum.Ignore,
            Modulate = Colors.Red
        };

        _label.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        AddChild(_label);
        
        ConnectSignals();
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
            
        }
        catch (Exception ex)
        {
            MainFile.Logger.Warn($"NRerollButton OnRelease error: {ex.Message}");
        }
    }
}