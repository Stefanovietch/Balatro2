using Balatro.BalatroCode.Relics;
using Balatro.BalatroCode.UI;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace Balatro.BalatroCode.Patches;

public class MerchantPatches
{
    [HarmonyPatch(typeof(MerchantInventory), "PopulateRelicEntries")]
    public static class MerchantInventoryPatch
    {
        [HarmonyPostfix]
        public static void Postfix(MerchantInventory __instance)
        {
            try
            {
                if (!__instance.Player.Relics.Contains(ModelDb.Relic<OverstockPlus>())) return;
                RelicRarity[] relicRarityArray = new RelicRarity[3]
                {
                    RelicFactory.RollRarity(__instance.Player),
                    RelicFactory.RollRarity(__instance.Player),
                    RelicRarity.Shop
                };
                foreach (RelicRarity rarity in relicRarityArray)
                    __instance.AddRelicEntry(new MerchantRelicEntry(rarity, __instance.Player));
            }
            catch (Exception ex)
            {
                MainFile.Logger.Warn($"MerchantInventory PopulateRelicEntries postfix error: {ex.Message}");
            }
        }
    }
    
    [HarmonyPatch(typeof(NMerchantInventory), "Initialize")]
    public static class MerchantRerollPatch
    {
        [HarmonyPostfix]
        public static void Postfix(NMerchantInventory __instance, MerchantInventory inventory)
        {
            try
            {
                var button = new NRerollButton();
                button.Name = "RerollButton";
                button.CustomMinimumSize = new Vector2(160, 48);
                button.Position = new Vector2(20, 20); // adjust to taste
                button.MouseFilter = Control.MouseFilterEnum.Stop;
                button.FocusMode = Control.FocusModeEnum.All;

                button.Initialize(inventory);

                var slotsContainer = __instance.GetNode<Control>("%SlotsContainer");
                slotsContainer.AddChild(button);
            }
            catch (Exception ex)
            {
                MainFile.Logger.Warn($"NMerchantInventory Initialize postfix error: {ex.Message}");
            }
        }
    }
    
    [HarmonyPatch(typeof(MerchantInventory), "PopulatePotionEntries")]
    public static class MerchantCharacterCardPatch
    {
        [HarmonyPrefix]
        public static void Prefix(MerchantInventory __instance)
        {
            try
            {
                IReadOnlyList<MerchantPotionEntry> newPotionEntries = new List<MerchantPotionEntry>();
                foreach (PotionModel potionModel in PotionFactory.CreateRandomPotionsOutOfCombat(__instance.Player, 3, __instance.Player.PlayerRng.Shops))
                    newPotionEntries.AddItem(new MerchantPotionEntry(potionModel.ToMutable(), __instance.Player));
                
                var method = AccessTools.Method(typeof(MerchantInventory), "PopulatePotionEntries");
                method?.Invoke(__instance, new object[] { newPotionEntries , false });
            }
            catch (Exception ex)
            {
                MainFile.Logger.Warn($"MerchantInventory CharacterCard postfix error: {ex.Message}");
            }
        }
    }
}