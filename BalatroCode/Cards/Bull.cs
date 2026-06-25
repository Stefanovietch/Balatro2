using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class Bull() : BalatroCard(2,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("blockPerGold", 10),
        ..MakeCalculatedBlock(0, (card, target) => card.Owner.Gold / this.DynamicVars["blockPerGold"].BaseValue)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Decimal num = await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.CalculatedBlock.Calculate(this.Owner.Creature),ValueProp.Move, play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars["blockPerGold"].UpgradeValueBy(-2);
    }
}
