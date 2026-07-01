using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Balatro.BalatroCode.Powers;

public class JugglerPower() : BalatroPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override Decimal ModifyHandDraw(Player player, Decimal count)
    {
        return player != this.Owner.Player ? count : count + this.Amount;
    }
}