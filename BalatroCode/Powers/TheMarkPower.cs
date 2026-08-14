using Balatro.BalatroCode.Powers;
using Balatro.BalatroCode.UI;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Powers;

public class TheMarkPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public BlindType BlindType => BlindType.TheMark;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var players = Owner.CombatState?.RunState.Players;
        if (players is null) return;
        foreach (var player in players)
        {
            var playerPlayerCombatState = player.PlayerCombatState;
            if (playerPlayerCombatState is null || player.Character is not Character.Balatro) continue;
            foreach (var card in playerPlayerCombatState.AllCards)
            {
                if (card.Type != CardType.Power) continue;
                await CardCmd.Afflict<BalatroMarked>(card, 3M);
            }
        }
    }

    public override async Task AfterCardEnteredCombat(CardModel card)
    {
        if (card.Owner.Character is not Character.Balatro || card.Affliction != null || card.Type != CardType.Power)
            return;
        await CardCmd.Afflict<BalatroMarked>(card, 3M);
    }

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        if (card.Affliction is not BalatroMarked)
        {
            modifiedCost = originalCost;
            return false;
        }

        modifiedCost = card.Affliction.Amount;
        return true;
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        var players = Owner.CombatState?.RunState.Players;
        if (players is null) return Task.CompletedTask;
        foreach (var player in players)
        {
            var playerPlayerCombatState = player.PlayerCombatState;
            if (playerPlayerCombatState is null || player.Character is not Character.Balatro) continue;
            foreach (var card in playerPlayerCombatState.AllCards.Where(c => c.Affliction is BalatroMarked))
                CardCmd.ClearAffliction(card);
        }

        return Task.CompletedTask;
    }
}

public sealed class BalatroMarked : BalatroAfflictions
{
}