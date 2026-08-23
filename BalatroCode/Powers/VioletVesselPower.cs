using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Powers;

public class VioletVesselPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public BlindType BlindType => BlindType.VioletVessel;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var hpDiff = Owner.MaxHp - Owner.CurrentHp;
        Owner.SetMaxHpInternal(Owner.MaxHp * 3);
        Owner.SetCurrentHpInternal(Owner.MaxHp - hpDiff);
        return base.AfterApplied(applier, cardSource);
    }
    
    public override Task AfterRemoved(Creature oldOwner)
    {
        var hpDiff = Owner.MaxHp - Owner.CurrentHp;
        Owner.SetMaxHpInternal(Owner.MaxHp / 3M);
        Owner.SetCurrentHpInternal(Owner.MaxHp - hpDiff);
        return base.AfterRemoved(oldOwner);
    }
}