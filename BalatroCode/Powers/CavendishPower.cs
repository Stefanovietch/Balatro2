using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Powers;

public class CavendishPower() : BalatroPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task BeforeAttack(AttackCommand command)
    {
        if (command.Attacker != Owner || !command.DamageProps.IsPoweredAttack())
            return Task.CompletedTask;
        var internalData = GetInternalData<Data>();
        if (internalData.commandToModify != null ||
            (command.ModelSource != null && !(command.ModelSource is CardModel)) ||
            !command.DamageProps.IsPoweredAttack())
            return Task.CompletedTask;
        internalData.commandToModify = command;
        internalData.amountWhenAttackStarted = Amount;
        return Task.CompletedTask;
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (Owner != dealer || !props.IsPoweredAttack())
            return base.ModifyDamageMultiplicative(target, amount, props, dealer, cardSource, cardPlay);
        ;
        var internalData = GetInternalData<Data>();
        return (internalData.commandToModify != null && cardSource != null &&
                cardSource != internalData.commandToModify.ModelSource) ||
               (internalData.commandToModify != null && internalData.commandToModify.Attacker != dealer)
            ? 1M
            : Amount;
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        var internalData = GetInternalData<Data>();
        if (command != internalData.commandToModify)
            return;
        var num = await PowerCmd.ModifyAmount(choiceContext, this, -internalData.amountWhenAttackStarted, null, null);
    }

    private class Data
    {
        public AttackCommand? commandToModify;
        public int amountWhenAttackStarted;
    }
}