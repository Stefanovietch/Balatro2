using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace Balatro.BalatroCode.UI;

public partial class NRerollGlutButton : NRerollButton
{
    private MerchantInventory? _inventory;
    private NMerchantInventory? _screen;
    
    public void Initialize(MerchantInventory inventory, NMerchantInventory screen)
    {
        Name = "RerollGlutButton";
        MouseFilter = MouseFilterEnum.Stop;
        FocusMode = FocusModeEnum.All;
        _inventory = inventory;
        _screen = screen;
    }
    
    protected override async void OnRelease()
    {
        if (_inventory == null || _screen == null) return;

        var player = _inventory.Player;
        if (player.Gold < RerollCost) return;

        await PlayerCmd.LoseGold(RerollCost, player, GoldLossType.Spent);

        RefreshCardSlots(_screen, _inventory);
    }
    
    private static void RefreshCardSlots(NMerchantInventory node, MerchantInventory inventory)
    {
        Traverse.Create(inventory).Field("_characterCardEntries").SetValue(new List<MerchantCardEntry>());
        Traverse.Create(inventory).Field("_colorlessCardEntries").SetValue(new List<MerchantCardEntry>());
        Traverse.Create(inventory).Method("PopulateCharacterCardEntries").GetValue();
        Traverse.Create(inventory).Method("PopulateColorlessCardEntries").GetValue();
        
        var charContainer = Traverse.Create(node).Field("_characterCardContainer").GetValue<Control>();
        for (int i = 0; i < inventory.CharacterCardEntries.Count; i++)
        {
            var child = charContainer.GetChild<NMerchantCard>(i);
            child.FillSlot(inventory.CharacterCardEntries[i]);
        }

        var colorlessContainer = Traverse.Create(node).Field("_colorlessCardContainer").GetValue<Control>();
        for (int i = 0; i < inventory.ColorlessCardEntries.Count; i++)
        {
            var child = colorlessContainer.GetChild<NMerchantCard>(i);
            child.FillSlot(inventory.ColorlessCardEntries[i]);
        }

        Traverse.Create(node).Method("SubscribeToEntries").GetValue();
        Traverse.Create(node).Method("UpdateNavigation").GetValue();
    }
}