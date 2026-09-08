using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
        new("DamageDecrease", 0.75M)
    ];

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource, 
        CardPlay? cardPlay)
    {
        if (dealer != Owner || !props.IsPoweredAttack())
            return 1M;
        var amount1 = DynamicVars["DamageDecrease"].BaseValue;
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
        var power = Owner.GetPower<WeakPower>();
        if (power != null) await PowerCmd.Remove(power);
    }

    public override bool TryModifyPowerAmountReceived(
        PowerModel canonicalPower,
        Creature target,
        decimal amount,
        Creature? _,
        out decimal modifiedAmount)
    {
        if (target != Owner || canonicalPower is not WeakPower || !canonicalPower.IsVisible)
        {
            modifiedAmount = amount;
            return false;
        }

        modifiedAmount = 0M;
        return true;
    }
}