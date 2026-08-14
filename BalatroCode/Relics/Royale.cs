using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Relics;

public class Royale() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount - DynamicVars.Energy.BaseValue;
    }

    public override bool TryModifyCardRewardOptions(
        Player player,
        List<CardCreationResult> options,
        CardCreationOptions creationOptions)
    {
        if (Owner != player) return false;
        IEnumerable<CardModel> cardModels = creationOptions.GetPossibleCards(player).Where(c =>
            options.TrueForAll((Predicate<CardCreationResult>)(o => o.originalCard.Id != c.Id))).ToArray();
        if (!cardModels.Any()) cardModels = creationOptions.GetPossibleCards(player).ToArray();
        if (!cardModels.Any()) return false;
        var cards = CardFactory.CreateForReward(Owner, 2,
            new CardCreationOptions(cardModels, CardCreationSource.Other, creationOptions.RarityOdds).WithFlags(
                CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications));
        foreach (var card in cards)
        {
            var cardCreationResult = new CardCreationResult(card.Card);
            cardCreationResult.ModifyCard(card.Card, this);
            options.Add(cardCreationResult);
        }

        return true;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        TaskHelper.RunSafely(ActivateVisuals());
        return Task.CompletedTask;
    }

    private async Task ActivateVisuals()
    {
        Flash();
        await Cmd.Wait(1f);
    }
}