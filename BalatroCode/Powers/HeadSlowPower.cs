using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Powers;

public class HeadSlowPower : BalatroPower
{
    public override PowerType Type =>
        PowerType.Debuff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override int DisplayAmount => DynamicVars["SlowAmount"].IntValue * 10;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("SlowAmount", 0M)
    ];

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player) return Task.CompletedTask;
        ++DynamicVars["SlowAmount"].BaseValue;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        return target != Owner || !props.IsPoweredAttack() ? 1M : 1M + 0.1M * DynamicVars["SlowAmount"].BaseValue;
    }

    public override Task AfterModifyingDamageAmount(CardModel? cardSource)
    {
        Flash();
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner))
            return Task.CompletedTask;
        DynamicVars["SlowAmount"].BaseValue = 0M;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}