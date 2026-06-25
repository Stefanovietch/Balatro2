using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers.Models;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Relics;

public class PlanetTycoon() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Common;

    
    public override bool IsAllowed(IRunState runState)
    {
        return RelicModel.IsBeforeAct3TreasureChest(runState);
    }

    public override bool TryModifyCardRewardOptionsLate(
        Player player,
        List<CardCreationResult> cardRewards,
        CardCreationOptions options)
    {
        if (player != this.Owner) return false;
        foreach (CardCreationResult cardReward in cardRewards)
        {
            CardModel card1 = cardReward.Card;
            if (card1.IsUpgradable && card1.Rarity == CardRarity.Common)
            {
                CardModel card2 = this.Owner.RunState.CloneCard(card1);
                CardCmd.Upgrade(card2);
                cardReward.ModifyCard(card2, (RelicModel) this);
            }
        }
        return true;
    }

    public override void ModifyMerchantCardCreationResults(
        Player player,
        List<CardCreationResult> cards)
    {
        if (player != this.Owner) return;
        foreach (CardCreationResult cardReward in cards)
        {
            CardModel card1 = cardReward.Card;
            if (card1.IsUpgradable && card1.Rarity == CardRarity.Common)
            {
                CardModel card2 = this.Owner.RunState.CloneCard(card1);
                CardCmd.Upgrade(card2);
                cardReward.ModifyCard(card2, (RelicModel) this);
            }
        }
    }

    public override bool TryModifyCardBeingAddedToDeck(CardModel card, out CardModel? newCard)
    {
        newCard = null;
        if (card.Owner != this.Owner || card.Rarity != CardRarity.Common || !card.IsUpgradable || card.CurrentUpgradeLevel >= 1)
            return false;
        newCard = this.Owner.RunState.CloneCard(card);
        CardCmd.Upgrade(newCard, CardPreviewStyle.None);
        return true;
    }
}