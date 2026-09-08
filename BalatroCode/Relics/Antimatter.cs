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
        return IsBeforeAct3TreasureChest(runState);
    }

    public override bool TryModifyCardRewardOptions(
        Player player,
        List<CardCreationResult> options,
        CardCreationOptions creationOptions)
    {
        if (Owner != player) return false;
        var card = CardFactory.CreateForReward(Owner, 1,
                new CardCreationOptions(creationOptions.CardPools, CardCreationSource.Other, creationOptions.RarityOdds).WithFlags(
                    CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications))
            .FirstOrDefault()?.Card;
        if (card == null) return false;
        var cardCreationResult = new CardCreationResult(card);
        cardCreationResult.ModifyCard(card, this);
        options.Add(cardCreationResult);
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