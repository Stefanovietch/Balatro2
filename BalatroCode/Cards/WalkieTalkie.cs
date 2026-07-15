using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Powers;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class WalkieTalkie() : BalatroCard(1,
    CardType.Power, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<WalkieTalkiePower>(2)

    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<WalkieTalkiePower>(choiceContext, this.Owner.Creature, this.DynamicVars.Power<WalkieTalkiePower>().BaseValue, this.Owner.Creature, this);

    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Power<WalkieTalkiePower>().UpgradeValueBy(1);

    }
}
