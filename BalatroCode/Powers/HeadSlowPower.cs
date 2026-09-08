using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Powers;

public class HeadSlowPower() : BalatroPower
{
    public override PowerType Type =>
        PowerType.Debuff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override int DisplayAmount => this.DynamicVars["SlowAmount"].IntValue * 10;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("SlowAmount", 0M)
    ];

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != this.Owner.Player) return Task.CompletedTask;
        ++this.DynamicVars["SlowAmount"].BaseValue;
        this.InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Decimal ModifyDamageMultiplicative(
        Creature? target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource, 
        CardPlay? cardPlay)
    {
        return target != this.Owner || !props.IsPoweredAttack() ? 1M : 1M + 0.1M * this.DynamicVars["SlowAmount"].BaseValue;
    }

    public override Task AfterModifyingDamageAmount(CardModel? cardSource)
    {
        this.Flash();
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(this.Owner))
            return Task.CompletedTask;
        this.DynamicVars["SlowAmount"].BaseValue = 0M;
        this.InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}