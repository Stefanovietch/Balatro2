using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rooms;

namespace Balatro.BalatroCode.Relics;

public class ROI() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not MerchantRoom merchantRoom || merchantRoom.GetLocalInventory().Player != this.Owner) 
        { 
            this.Status = RelicStatus.Normal; 
            return Task.CompletedTask;
        }
        this.Flash();
        this.Status = RelicStatus.Active;

        var inventory = merchantRoom.GetLocalInventory();
        
        //Upgrade all unupgraded cards in the shop
        List<CardModel> cardModels = [];
        foreach (MerchantCardEntry merchantCardEntry in inventory.CardEntries)
        {
            if (merchantCardEntry.CreationResult == null) continue;
            if (!merchantCardEntry.CreationResult.Card.IsUpgradable) continue;
            cardModels.Add(merchantCardEntry.CreationResult.Card);
        }
        CardCmd.Upgrade(cardModels, CardPreviewStyle.None);

        //Check if rare card already exist
        bool rareCard = inventory.CharacterCardEntries
            .Any(e => e.CreationResult?.Card.Rarity == CardRarity.Rare);
        
        //Create new random card if none existed, keeping card type
        if (!rareCard)
        {
            try
            {
                var fieldCharacterCardEntries = typeof(MerchantInventory)
                    .GetField("_characterCardEntries", BindingFlags.NonPublic | BindingFlags.Instance);
                var fieldCardList =
                    (List<MerchantCardEntry>)fieldCharacterCardEntries!.GetValue(inventory);

                List<CardModel> cardList = this.Owner.Character.CardPool
                    .GetUnlockedCards(this.Owner.UnlockState, this.Owner.RunState.CardMultiplayerConstraint)
                    .ToList<CardModel>().Where(c => c.Rarity == CardRarity.Rare).ToList();
                int cardNum =
                    this.Owner.PlayerRng.Shops.NextInt(0, inventory.CharacterCardEntries.Count - 1);
                CardCreationResult? cardCreationResult =
                    inventory.CharacterCardEntries[cardNum].CreationResult;
                if (cardCreationResult != null)
                {
                    var newEntry = new MerchantCardEntry(
                        this.Owner,
                        inventory,
                        cardList,
                        cardCreationResult.Card.Type
                    );
                    newEntry.Populate();
                    if (fieldCardList != null) fieldCardList[cardNum] = newEntry;
                }
            }
            catch (Exception ex)
            {
                MainFile.Logger.Warn($"ROI rare card error: {ex.Message}");
            }

        }
        
        //Replace all common potion with a random one of higher rarity
        List<MerchantPotionEntry> newPotions = [];
        foreach (MerchantPotionEntry merchantPotionEntry in inventory.PotionEntries)
        {
            MerchantPotionEntry newPotionEntry = merchantPotionEntry;
            if (newPotionEntry.Model is { Rarity: PotionRarity.Common })
            {
                var newModel = PotionFactory.CreateRandomPotionsOutOfCombat(this.Owner, 1, this.Owner.PlayerRng.Shops,this.Owner.Character.PotionPool.GetUnlockedPotions(this.Owner.UnlockState).Concat<PotionModel>(ModelDb.PotionPool<SharedPotionPool>().GetUnlockedPotions(this.Owner.UnlockState)).Where(c => c.Rarity == PotionRarity.Common)).First();
                newPotionEntry = new MerchantPotionEntry(newModel.ToMutable(), this.Owner);
            }
            newPotions.Add(newPotionEntry);
        }

        try
        {
            var fieldPotionEntries = typeof(MerchantInventory)
                .GetField("_potionEntries", BindingFlags.NonPublic | BindingFlags.Instance);
            var potionList = (List<MerchantPotionEntry>)fieldPotionEntries!.GetValue(inventory);
            if (potionList != null)
            {
                potionList.Clear();
                potionList.AddRange(newPotions);
            }
        }
        catch (Exception ex)
        {
            MainFile.Logger.Warn($"ROI potion error: {ex.Message}");
        }

        return Task.CompletedTask;
    }
}