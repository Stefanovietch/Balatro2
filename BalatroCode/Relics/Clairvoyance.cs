using Balatro.BalatroCode.Relics;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;

namespace Balatro.BalatroCode.Relics;

public class Clairvoyance() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;
    
    public override bool HasUponPickupEffect => true;

    private bool _usedUp = false;

    public override async Task AfterActEntered()
    {
        if (this._usedUp) return;
        await ColorlessCardReward();
        _usedUp = true;
    }

    public override async Task AfterObtained()
    {
        await PlayerCmd.LoseMaxPotionCount(1, this.Owner);
        Owner.RelicGrabBag.Remove(ModelDb.Relic<DingyRug>());
        if (this._usedUp || this.Owner.Relics.Count <= 1) return;
        await ColorlessCardReward();
        _usedUp = true;
    }

    private async Task ColorlessCardReward()
    {
        CardCreationOptions options1 = new CardCreationOptions(new List<CardPoolModel> { ModelDb.CardPool<ColorlessCardPool>() }, CardCreationSource.Other, CardRarityOddsType.Uniform, (Func<CardModel, bool>) (c => c.Rarity == CardRarity.Rare)).WithFlags(CardCreationFlags.NoRarityModification);
        List<CardModel> options = CardFactory.CreateForReward(this.Owner, 3, options1).Select<CardCreationResult, CardModel>(r => r.Card).ToList();
        CardModel? chosenCard = await CardSelectCmd.FromChooseACardScreen(new BlockingPlayerChoiceContext(), options, this.Owner, true);
        if (chosenCard != null) CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(chosenCard, PileType.Deck));
        await SaveManager.Instance.SaveRun(this.Owner.RunState.CurrentRoom);
    }
    

    public override CardCreationOptions ModifyCardRewardCreationOptions(
        Player player,
        CardCreationOptions options)
    {
        if (Owner != player || options.Flags.HasFlag(CardCreationFlags.NoCardPoolModifications))
            return options;
        var list1 = options.GetPossibleCards(player).ToList();
        var list2 = ModelDb.CardPool<ColorlessCardPool>()
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint).ToList();
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