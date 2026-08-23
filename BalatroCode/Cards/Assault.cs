using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class Assault() : BalatroTokenCard(1,
    CardType.Attack,
    TargetType.AllEnemies), IObeliskOption
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4, ValueProp.Move)
    ];

    public void UpdateValue(CardModel card)
    {
        DynamicVars["DamageIncrease"].BaseValue = card.DynamicVars["DamageIncrease"].BaseValue;
        DynamicVars["BlockIncrease"].BaseValue = card.DynamicVars["BlockIncrease"].BaseValue;
    }
}