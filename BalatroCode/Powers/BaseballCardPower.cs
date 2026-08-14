using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Powers;

public class BaseballCardPower() : BalatroPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.None;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (cardSource?.Rarity != CardRarity.Uncommon)
            return base.ModifyDamageMultiplicative(target, amount, props, dealer, cardSource);

        var multiplier = (decimal)(Amount * 0.1);
        return base.ModifyDamageMultiplicative(target, amount * multiplier, props, dealer, cardSource);
    }

    public override int DisplayAmount => (int)(Amount * 0.1);
}