using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Powers;

public class VioletVesselPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public BlindType BlindType => BlindType.VioletVessel;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var hpDiff = Owner.MaxHp - Owner.CurrentHp;
        Owner.SetMaxHpInternal(Owner.MaxHp * 3);
        Owner.SetCurrentHpInternal(Owner.MaxHp - hpDiff);
    }
    
    public override async Task AfterRemoved(Creature oldOwner)
    {
        if (this.Owner.IsDead) return;
        var hpDiff = Owner.MaxHp - Owner.CurrentHp;
        Owner.SetMaxHpInternal(Owner.MaxHp / 3M);
        var newHp = Owner.MaxHp - hpDiff < 0 ? 0 : Owner.MaxHp - hpDiff;
        Owner.SetCurrentHpInternal(newHp);
    }
    
    public override decimal ModifyPowerAmountGivenAdditive(PowerModel power, Creature giver, decimal amount, Creature? target,
        CardModel? cardSource)
    {
        if (Owner.Monster is CeremonialBeast && power is PlowPower)
            return amount * 2;
        return base.ModifyPowerAmountGivenAdditive(power, giver, amount, target, cardSource);
    }
}