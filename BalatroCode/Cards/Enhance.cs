using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Cards;

public class Enhance() : BalatroTokenCard(1,
    CardType.Power, TargetType.Self), IObeliskOption
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new IntVar("DamageIncrease", 1M),
        new IntVar("BlockIncrease", 2M)
    ];

    public void UpdateValue(CardModel card)
    {
        this.DynamicVars.Damage.BaseValue = card.DynamicVars.Damage.BaseValue;
    }
}