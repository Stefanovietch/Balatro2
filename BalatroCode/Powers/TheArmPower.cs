using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Powers;

public class TheArmPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;
    
    public BlindType BlindType => BlindType.TheArm;
    
    public override bool TryModifyEnergyCostInCombatLate(CardModel card, Decimal originalCost, out Decimal modifiedCost)
    {
        if (card.IsUpgraded)
        {
            modifiedCost = originalCost + 1;
            return true;
        }
        modifiedCost = originalCost;
        return false;
    }
}