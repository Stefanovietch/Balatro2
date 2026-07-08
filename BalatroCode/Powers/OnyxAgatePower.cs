using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Powers;

public class OnyxAgatePower : TemporaryStrengthPower, ICustomPower
{
    public override AbstractModel OriginModel => (AbstractModel) ModelDb.Card<SetupStrike>();
}
