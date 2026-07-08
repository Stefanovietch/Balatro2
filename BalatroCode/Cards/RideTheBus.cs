using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class RideTheBus() : BalatroCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(4),
        new ExtraDamageVar(1),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) =>
        {
            var cardsPlayed= CombatManager.Instance.History.CardPlaysFinished.Where(c => c.CardPlay.Card.Owner == card.Owner).ToList();
            int lastPowerIndex = cardsPlayed.FindLastIndex(c => c.CardPlay.Card.Type == CardType.Power);
            return lastPowerIndex == -1 ? cardsPlayed.Count : cardsPlayed.Count - lastPowerIndex - 1;
        })
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(this.DynamicVars.CalculatedDamage.Calculate(play.Target)).FromCard(this)
            .Targeting(play.Target)
            .WithHitFx(null, null, "blunt_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.ExtraDamage.UpgradeValueBy(1M);
    }
}
