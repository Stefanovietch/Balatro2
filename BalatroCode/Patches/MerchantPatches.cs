using System.Reflection;
using Balatro.BalatroCode.Relics;
using Balatro.BalatroCode.UI;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace Balatro.BalatroCode.Patches;

public class MerchantPatches
{
    [HarmonyPatch(typeof(MerchantInventory), "PopulateRelicEntries")]
    public static class MerchantRelicPatch
    {
        [HarmonyPostfix]
        public static void Postfix(
            MerchantInventory __instance,
            List<MerchantRelicEntry> ____relicEntries)
        {
            if (__instance.Player.GetRelic<OverstockPlus>() == null) return;
            
            var relicRarityArray = new RelicRarity[3]
            {
                RelicFactory.RollRarity(__instance.Player),
                RelicFactory.RollRarity(__instance.Player),
                RelicRarity.Shop
            };
            foreach (var rarity in relicRarityArray)
            {
                var mre = new MerchantRelicEntry(rarity, __instance.Player);
                ____relicEntries.Add(mre);
            }
        }
    }
    

    [HarmonyPatch(typeof(NMerchantInventory), "Initialize")]
    public static class MerchantRerollPatch
    {
        [HarmonyPrefix]
        public static void Prefix(
            NMerchantInventory __instance,
            MerchantInventory inventory)
        {
            if (inventory.Player.GetRelic<OverstockPlus>() != null)
            {

                var potionContainer = __instance.GetNode<Control>("%Potions");

                potionContainer.GetChild<NMerchantPotion>(0).Position += Vector2.Down * 144;
                potionContainer.GetChild<NMerchantPotion>(1).Position += Vector2.Down * 144;
                potionContainer.GetChild<NMerchantPotion>(2).Position += Vector2.Down * 144;

                var relicContainer = __instance.GetNode<Control>("%Relics");

                for (int i = 0; i < 3; i++)
                {
                    var source = relicContainer.GetChild<NMerchantRelic>(i);

                    var clone = source.Duplicate(
                        (int)Node.DuplicateFlags.UseInstantiation
                    ) as NMerchantRelic;

                    if (clone == null)
                        continue;

                    relicContainer.AddChild(clone);

                    clone.Position = new Vector2(
                        source.Position.X,
                        source.Position.Y + 144
                    );
                }
            }
            
            var slotsContainer = __instance.GetNode<Control>("%SlotsContainer");

            //if (inventory.Player.GetRelic<RerollGlut>() != null) {
            var button = new NRerollButton();
            button.SetAnchorsAndOffsetsPreset(
                Control.LayoutPreset.TopLeft,
                Control.LayoutPresetMode.KeepSize,
                10);

            button.Position = new Vector2(200, 200);
            button.Size = new Vector2(100, 100);
            button.ZIndex = 100;
            
            slotsContainer.AddChild(button);
            button.Initialize(inventory, __instance);
            //}
        }
    }

    [HarmonyPatch(typeof(MerchantInventory), "PopulatePotionEntries")]
    public static class MerchantPotionPatch
    {
        [HarmonyPostfix]
        public static void Postfix(
            MerchantInventory __instance,
            List<MerchantPotionEntry> ____potionEntries)
        {
            if (__instance.Player.GetRelic<ROI>() == null)
                return;
            var potionList = __instance.Player.Character.PotionPool.GetUnlockedPotions(__instance.Player.UnlockState).Concat(ModelDb.PotionPool<SharedPotionPool>().GetUnlockedPotions(__instance.Player.UnlockState)).Where(c => c.Rarity != PotionRarity.Common).ToList();
 
            for (int i = 0; i < ____potionEntries.Count; i++)
            {
                if (____potionEntries[i].Model?.Rarity != PotionRarity.Common)
                    continue;
                
                var potion = __instance.Player.PlayerRng.Shops.NextItem(potionList)?.ToMutable();
                if (potion == null) continue;
                
                ____potionEntries[i] = new MerchantPotionEntry(
                    potion,
                    __instance.Player
                );
            }
        }
    }
    
    [HarmonyPatch(typeof(MerchantInventory), "PopulateCharacterCardEntries")]
    public static class MerchantCardPatch
    {
        [HarmonyPostfix]
        public static void Postfix(
            MerchantInventory __instance,
            List<MerchantCardEntry> ____characterCardEntries)
        {
            if (__instance.Player.GetRelic<ROI>() == null) return;
            
            bool hasRare = false;
            
            foreach (var mce in ____characterCardEntries)
            {
                var card = mce.CreationResult?.Card;
                if (card?.Rarity == CardRarity.Rare) hasRare = true;
                if (card == null || card.IsUpgraded) continue;
                card.UpgradeInternal();
            }

            if (hasRare) return;
            var toReplace=  __instance.Player.PlayerRng.Shops.NextInt(0, ____characterCardEntries.Count);
            List<CardModel> cardPool = __instance.Player.Character.CardPool.GetUnlockedCards(__instance.Player.UnlockState, __instance.Player.RunState.CardMultiplayerConstraint).ToList();
            var replacement = new MerchantCardEntry(__instance.Player, __instance, cardPool, CardRarity.Rare);
            replacement.Populate();
            replacement.CreationResult?.Card.UpgradeInternal();
            ____characterCardEntries[toReplace] = replacement;
        }
    }
    
    [HarmonyPatch(typeof(MerchantInventory), "PopulateColorlessCardEntries")]
    public static class MerchantColorlessCardPatch
    {
        [HarmonyPostfix]
        public static void Postfix(
            MerchantInventory __instance,
            List<MerchantCardEntry> ____colorlessCardEntries)
        {
            if (__instance.Player.GetRelic<ROI>() == null) return;
            
            foreach (var mce in ____colorlessCardEntries)
            {
                var card = mce.CreationResult?.Card;
                if (card == null || card.IsUpgraded) continue;
                card.UpgradeInternal();
            }
        }
    }
}