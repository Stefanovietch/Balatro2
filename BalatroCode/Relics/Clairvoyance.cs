using Balatro.BalatroCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;

namespace Balatro.BalatroCode.Relics;

public class Clairvoyance() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override async Task AfterObtained()
    {
        await PlayerCmd.LoseMaxPotionCount(1, this.Owner);
        Owner.RelicGrabBag.Remove(ModelDb.Relic<DingyRug>());
        
        var task = TaskHelper.RunSafely(OfferColorlessRareChoice());
        MainFile.Logger.Info("check 0: " + task);
    }
    
    private async Task OfferColorlessRareChoice()
    {
        while (!LocalContext.IsMe(Owner))
            await Cmd.Wait(1);
        
        MainFile.Logger.Info("check 1");
        var options = CardCreationOptions
            .ForNonCombatWithUniformOdds(
                new List<CardPoolModel> { ModelDb.CardPool<ColorlessCardPool>() },
                (Func<CardModel, bool>)(c => c.Rarity == CardRarity.Rare)
            )
            .WithFlags(CardCreationFlags.NoRarityModification);

        var rewards = new List<Reward> { new CardReward(options, 3, Owner) };
        MainFile.Logger.Info("check 2");        
        await RewardsCmd.OfferCustom(Owner, rewards);
        MainFile.Logger.Info("check 3");
    }

    public override CardCreationOptions ModifyCardRewardCreationOptions(
        Player player,
        CardCreationOptions options)
    {
        if (Owner != player || options.Flags.HasFlag(CardCreationFlags.NoCardPoolModifications))
            return options;
        var list1 = options.GetPossibleCards(player).ToList();
        var list2 = ModelDb.CardPool<ColorlessCardPool>()
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint).ToList<CardModel>();
        if (options.Flags.HasFlag(CardCreationFlags.NoRarityModification))
        {
            var allowedRarities = options.GetPossibleCards(player)
                .Select((c => c.Rarity)).ToHashSet();
            list2 = list2.Where((c => allowedRarities.Contains(c.Rarity)))
                .ToList();
        }

        foreach (var cardModel in list2)
            if (!list1.Contains(cardModel))
                list1.Add(cardModel);
        return options.WithCustomPool(list1);
    }
}