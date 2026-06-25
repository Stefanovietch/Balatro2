using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Relics;

public class Antimatter() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;
    
    public override bool IsAllowed(IRunState runState)
    {
        return RelicModel.IsBeforeAct3TreasureChest(runState);
    }

    public override bool TryModifyCardRewardOptions(
        Player player,
        List<CardCreationResult> options,
        CardCreationOptions creationOptions)
    {
        if (this.Owner != player) return false;
        IEnumerable<CardModel> cardModels = creationOptions.GetPossibleCards(player).Where(c => options.TrueForAll((Predicate<CardCreationResult>) (o => o.originalCard.Id != c.Id))).ToArray();
        if (!cardModels.Any()) cardModels = creationOptions.GetPossibleCards(player).ToArray();
        if (!cardModels.Any()) return false;
        CardModel? card = CardFactory.CreateForReward(this.Owner, 1, new CardCreationOptions(cardModels, CardCreationSource.Other, creationOptions.RarityOdds).WithFlags(CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications)).FirstOrDefault<CardCreationResult>()?.Card;
        if (card == null) return false;
        CardCreationResult cardCreationResult = new CardCreationResult(card);
        cardCreationResult.ModifyCard(card, this);
        options.Add(cardCreationResult);
        return true;
    }
    
    public override Task AfterCombatEnd(CombatRoom room)
    {
        TaskHelper.RunSafely(this.ActivateVisuals());
        return Task.CompletedTask;
    }
    
    private async Task ActivateVisuals()
    {
        this.Flash();
        await Cmd.Wait(1f);
    }
}