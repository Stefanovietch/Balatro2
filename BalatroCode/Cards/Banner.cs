using Balatro.BalatroCode.Cards;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class Banner() : BalatroCard(2,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        ..MakeCalculatedBlock(24, (card, _) => -PileType.Discard.GetPile(card.Owner).Cards.Count)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.CalculatedBlock.Calculate(null), ValueProp.Move , play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.CalculationBase.UpgradeValueBy(7);
    }
}
