using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Relics;

public class Clairvoyance() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override async Task AfterObtained()
    {
        Owner.SubtractFromMaxPotionCount(1);
        Owner.RelicGrabBag.Remove(ModelDb.Relic<DingyRug>());

        var rewards = new List<Reward>();
        // ISSUE: object of a compiler-generated type is created
        var options = CardCreationOptions
            .ForNonCombatWithUniformOdds(
                new List<CardPoolModel> { ModelDb.CardPool<ColorlessCardPool>() },
                (Func<CardModel, bool>)(c => c.Rarity == CardRarity.Rare)
            )
            .WithFlags(CardCreationFlags.NoRarityModification);
        rewards.Add(new CardReward(options, 3, Owner));

        await RewardsCmd.OfferCustom(Owner, rewards);
    }

    public override CardCreationOptions ModifyCardRewardCreationOptions(
        Player player,
        CardCreationOptions options)
    {
        if (Owner != player || options.Flags.HasFlag((Enum)CardCreationFlags.NoCardPoolModifications))
            return options;
        var list1 = options.GetPossibleCards(player).ToList<CardModel>();
        var list2 = ModelDb.CardPool<ColorlessCardPool>()
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint).ToList<CardModel>();
        if (options.Flags.HasFlag((Enum)CardCreationFlags.NoRarityModification))
        {
            var allowedRarities = options.GetPossibleCards(player)
                .Select<CardModel, CardRarity>((Func<CardModel, CardRarity>)(c => c.Rarity)).ToHashSet<CardRarity>();
            list2 = list2.Where<CardModel>((Func<CardModel, bool>)(c => allowedRarities.Contains(c.Rarity)))
                .ToList<CardModel>();
        }

        foreach (var cardModel in list2)
            if (!list1.Contains(cardModel))
                list1.Add(cardModel);
        return options.WithCustomPool((IEnumerable<CardModel>)list1);
    }
}