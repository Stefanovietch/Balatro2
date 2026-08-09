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
        var hpDiff = this.Owner.MaxHp - this.Owner.CurrentHp;
        this.Owner.SetMaxHpInternal(this.Owner.MaxHp * 2);
        this.Owner.SetCurrentHpInternal(this.Owner.MaxHp * 2 - hpDiff);
        return base.AfterApplied(applier, cardSource);
    }
}