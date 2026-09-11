using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class ShootTheMoon() : BalatroCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, ValueProp.Move),
        new CalculationBaseVar(0M),
        new CalculationExtraVar(1M),
        new CalculatedVar("CalculatedHits").WithMultiplier((card, _) =>
            PileType.Hand.GetPile(card.Owner).Cards.Count(c => c.Type == CardType.Skill))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .WithHitCount((int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(play.Target))
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}