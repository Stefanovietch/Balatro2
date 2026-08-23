using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Powers;

public class TopUpPower() : BalatroPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task BeforeAttack(AttackCommand command)
    {
        if (command.Attacker?.Player?.Character is not Character.Balatro) return;
        await CreatureCmd.GainBlock(this.Owner, this.Amount, ValueProp.Move, null);
    }
}