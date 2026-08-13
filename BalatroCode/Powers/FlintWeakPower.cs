using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Powers;

public class FlintWeakPower() : BalatroPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("DamageDecrease", 0.75M),
    ];

    public override Decimal ModifyDamageMultiplicative(
        Creature? target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer != this.Owner || !props.IsPoweredAttack())
            return 1M;
        Decimal amount1 = this.DynamicVars["DamageDecrease"].BaseValue;
        var relic = target?.Player?.GetRelic<PaperKrane>();
        if (relic != null && target != null)
            amount1 = relic.ModifyWeakMultiplier(target, amount1, props, dealer, cardSource);
        var power = dealer.GetPower<DebilitatePower>();
        if (power != null)
            amount1 = power.ModifyWeakMultiplier(dealer, amount1, props, dealer, cardSource);
        return amount1;
    }
    
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var power = this.Owner.GetPower<WeakPower>();
        if (power != null) await PowerCmd.Remove(power);
    }

    public override bool TryModifyPowerAmountReceived(
        PowerModel canonicalPower,
        Creature target,
        Decimal amount,
        Creature? _,
        out Decimal modifiedAmount)
    {
        if (target != this.Owner || canonicalPower is not WeakPower || !canonicalPower.IsVisible)
        {
            modifiedAmount = amount;
            return false;
        }
        modifiedAmount = 0M;
        return true;
    }
}