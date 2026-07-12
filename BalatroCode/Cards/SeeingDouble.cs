using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Powers;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Cards;

public class SeeingDouble() : BalatroCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<SeeingDoublePower>(1),
        new PowerVar<ConfusedPower>(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<SeeingDoublePower>(choiceContext, this.Owner.Creature, this.DynamicVars.Power<SeeingDoublePower>().BaseValue, this.Owner.Creature, this);
        await PowerCmd.Apply<ConfusedPower>(choiceContext, this.Owner.Creature, this.DynamicVars.Power<ConfusedPower>().BaseValue, this.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        this.EnergyCost.UpgradeBy(-1);
    }
}
