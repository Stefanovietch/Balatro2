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
        var hpDiff = this.Owner.MaxHp - this.Owner.CurrentHp;
        this.Owner.SetMaxHpInternal(this.Owner.MaxHp * 3);
        this.Owner.SetCurrentHpInternal(this.Owner.MaxHp * 3 - hpDiff);
        return base.AfterApplied(applier, cardSource);
    }
}