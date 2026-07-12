using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class SmileyFace() : BalatroCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3, ValueProp.Move),
        new CalculationBaseVar(0M),
        new CalculationExtraVar(1M),
        new CalculatedVar("PowersPlayed").WithMultiplier((card, _) =>  CombatManager.Instance.History.CardPlaysFinished.Count(c => c.CardPlay.Card.Type == CardType.Power && c.CardPlay.Card.Owner == card.Owner))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this)
            .WithHitCount((int)((CalculatedVar)base.DynamicVars["PowersPlayed"]).Calculate(play.Target))
            .Targeting(play.Target)
            .WithHitFx()
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(2);
    }
}
