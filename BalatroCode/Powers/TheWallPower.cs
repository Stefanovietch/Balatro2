using Balatro.BalatroCode.Powers;
using BaseLib.Hooks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Powers;

public class TheWallPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public BlindType BlindType => BlindType.TheWall;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var hpDiff = Owner.MaxHp - Owner.CurrentHp;
        Owner.SetMaxHpInternal(Owner.MaxHp * 2);
        Owner.SetCurrentHpInternal(Owner.MaxHp - hpDiff);
        return base.AfterApplied(applier, cardSource);
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        if (this.Owner.IsDead) return base.AfterRemoved(oldOwner);;
        var hpDiff = Owner.MaxHp - Owner.CurrentHp;
        Owner.SetMaxHpInternal(Owner.MaxHp / 2M);
        var newHp = Owner.MaxHp - hpDiff < 0 ? 0 : Owner.MaxHp - hpDiff;
        Owner.SetCurrentHpInternal(newHp);
        return base.AfterRemoved(oldOwner);
    }
}