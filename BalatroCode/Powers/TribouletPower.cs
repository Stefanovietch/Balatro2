using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Powers;

public class TribouletPower() : BalatroPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public override decimal ModifyBlockMultiplicative(Creature target, decimal block, ValueProp props, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource?.Owner == this.Owner.Player && cardSource?.EnergyCost.GetAmountToSpend() == 2) return 2;
        return base.ModifyBlockMultiplicative(target, block, props, cardSource, cardPlay);
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        if (cardSource?.Owner == this.Owner.Player && cardSource?.EnergyCost.GetAmountToSpend() == 2) return 2;
        return base.ModifyDamageMultiplicative(target, amount, props, dealer, cardSource);
    }
}