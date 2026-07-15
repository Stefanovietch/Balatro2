using Balatro.BalatroCode.Cards;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Powers;

public class EBallPower : TemporaryStrengthPower, ICustomPower
{
    public override AbstractModel OriginModel => (AbstractModel) ModelDb.Card<EBall>();
    
    protected override bool IsPositive => false;
}
