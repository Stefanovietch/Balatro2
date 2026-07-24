using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Balatro.BalatroCode.Powers;


public class CeruleanBellPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public BlindType BlindType => BlindType.CeruleanBell;
}