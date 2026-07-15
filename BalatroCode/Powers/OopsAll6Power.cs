using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Balatro.BalatroCode.Powers;

public class OopsAll6Power() : BalatroPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override int DisplayAmount => (int) Math.Pow(2, Amount);
}