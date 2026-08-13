using Balatro.BalatroCode.Powers;
using Balatro.BalatroCode.UI;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;

namespace Balatro.BalatroCode.Powers;

public class TheArmPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;
    
    public BlindType BlindType => BlindType.TheArm;
    
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var players = this.Owner.CombatState?.RunState.Players;
        if (players is null) return;
        foreach (var player in players)
        {
            var playerPlayerCombatState = player.PlayerCombatState;
            if (playerPlayerCombatState is null || player.Character is not Character.Balatro) continue;
            foreach (CardModel card in playerPlayerCombatState.AllCards)
            {
                if (!card.IsUpgraded) continue;
                await CardCmd.Afflict<BalatroArmed>(card, 1M);
            }
        }
    }

    public override async Task AfterCardEnteredCombat(CardModel card)
    {
        if (card.Owner.Character is not Character.Balatro || card.Affliction != null || !card.IsUpgraded)
            return;
        await CardCmd.Afflict<BalatroArmed>(card, 1M);
    }
    
    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        Decimal originalCost,
        out Decimal modifiedCost)
    {
        if (card.Affliction is not BalatroArmed)
        {
            modifiedCost = originalCost;
            return false;
        }
        modifiedCost = originalCost + 1;
        return true;
    }
    
    public override Task AfterRemoved(Creature oldOwner)
    {
        var players = this.Owner.CombatState?.RunState.Players;
        if (players is null) return Task.CompletedTask;
        foreach (var player in players)
        {
            var playerPlayerCombatState = player.PlayerCombatState;
            if (playerPlayerCombatState is null || player.Character is not Character.Balatro) continue;
            foreach (CardModel card in playerPlayerCombatState.AllCards.Where(c => c.Affliction is BalatroArmed))
                CardCmd.ClearAffliction(card);
        }
        return Task.CompletedTask;
    }
}

public sealed class BalatroArmed : BalatroAfflictions
{
}