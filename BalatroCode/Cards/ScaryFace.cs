using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Powers;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class ScaryFace() : BalatroCard(1,
    CardType.Power, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ScaryFacePower>(8)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<ScaryFacePower>(choiceContext, this.Owner.Creature, this.DynamicVars.Power<ScaryFacePower>().BaseValue, this.Owner.Creature, this);

    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Power<ScaryFacePower>().UpgradeValueBy(3);
    }
}
