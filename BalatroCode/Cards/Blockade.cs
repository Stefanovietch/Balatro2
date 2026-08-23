using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class Blockade() : BalatroTokenCard(1,
    CardType.Skill, TargetType.Self), IObeliskOption
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8, ValueProp.Move)
    ];

    public void UpdateValue(CardModel card)
    {
        this.DynamicVars.Block.BaseValue = card.DynamicVars.Block.BaseValue;
    }
}