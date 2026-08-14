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

public class FlintVulnerablePower : BalatroPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("DamageIncrease", 1.5M)
    ];

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || !props.IsPoweredAttack())
            return 1M;
        var amount1 = DynamicVars["DamageIncrease"].BaseValue;
        if (dealer != null)
        {
            var relic = dealer.Player?.GetRelic<PaperPhrog>();
            if (relic != null)
                amount1 = relic.ModifyVulnerableMultiplier(target, amount1, props, dealer, cardSource);
            var power = dealer.GetPower<CrueltyPower>();
            if (power != null)
                amount1 = power.ModifyVulnerableMultiplier(target, amount1, props, dealer, cardSource);
        }

        var power1 = target.GetPower<DebilitatePower>();
        if (power1 != null)
            amount1 = power1.ModifyVulnerableMultiplier(target, amount1, props, dealer, cardSource);
        return amount1;
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var power = Owner.GetPower<VulnerablePower>();
        if (power != null) await PowerCmd.Remove(power);
    }

    public override bool TryModifyPowerAmountReceived(
        PowerModel canonicalPower,
        Creature target,
        decimal amount,
        Creature? _,
        out decimal modifiedAmount)
    {
        if (target != Owner || canonicalPower is not VulnerablePower || !canonicalPower.IsVisible)
        {
            modifiedAmount = amount;
            return false;
        }

        modifiedAmount = 0M;
        return true;
    }
}