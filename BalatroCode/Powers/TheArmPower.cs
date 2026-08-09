using Balatro.BalatroCode.Powers;
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
        var playerPlayerCombatState = this.Owner.Player?.PlayerCombatState;
        if (playerPlayerCombatState is not null && this.Owner.Player?.Character is Character.Balatro)
            foreach (CardModel card in playerPlayerCombatState.AllCards)
            {
                if (!card.IsUpgraded) continue;
                await CardCmd.Afflict<Armed>(card, 1M);
            }
    }

    public override async Task AfterCardEnteredCombat(CardModel card)
    {
        if (this.Owner.Player?.Character is not Character.Balatro || card.Affliction != null || !card.IsUpgraded)
            return;
        await CardCmd.Afflict<Armed>(card, 1M);
    }
    
    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        Decimal originalCost,
        out Decimal modifiedCost)
    {
        if (card.Affliction is not Armed || card.Owner != this.Owner.Player)
        {
            modifiedCost = originalCost;
            return false;
        }
        modifiedCost = originalCost + 1;
        return true;
    }
    
    public override Task AfterRemoved(Creature oldOwner)
    {
        var playerPlayerCombatState = this.Owner.Player?.PlayerCombatState;
        if (playerPlayerCombatState is not null)
            foreach (CardModel card in playerPlayerCombatState.AllCards.Where(c => c.Affliction is Armed))
                CardCmd.ClearAffliction(card);
        return Task.CompletedTask;
    }
}

public sealed class Armed : AfflictionModel
{
}