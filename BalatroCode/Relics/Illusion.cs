using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Relics;

public class Illusion() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Shop;

    public override async Task AfterObtained()
    {
        var rewards = new List<Reward>();
        var cardRarityArray = new CardRarity[3]
        {
            CardRarity.Common,
            CardRarity.Uncommon,
            CardRarity.Rare
        };
        foreach (var cardRarity in cardRarityArray)
        {
            var rarity = cardRarity;
            var options = CardCreationOptions.ForNonCombatWithUniformOdds(
                    new List<CardPoolModel> { Owner.Character.CardPool },
                    (Func<CardModel, bool>)(c => c.Rarity == rarity))
                .WithFlags(CardCreationFlags.NoRarityModification);
            rewards.Add((Reward)new CardReward(options, 3, Owner));
        }

        await RewardsCmd.OfferCustom(Owner, rewards);
    }
}