using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Powers;
using BaseLib.Abstracts;
using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;

namespace Balatro.BalatroCode.Cards;

public class EBall() : BalatroCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.AnyEnemy), IChance
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("Chance", 4),
        new DisplayVar<EBall>("Numerator", card => card
            .GetNumerator(card.IsCanonical ? null : card.Owner).ToString()),
        new PowerVar<EBallPower>(8)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await PowerCmd.Apply<EBallPower>(choiceContext, play.Target, DynamicVars.Power<EBallPower>().BaseValue,
            play.Card.Owner.Creature, this);
        if (this.RollChance(Owner, DynamicVars["Chance"].IntValue))
            await PotionCmd.TryToProcure(
                PotionFactory.CreateRandomPotionInCombat(Owner, Owner.RunState.Rng.CombatPotionGeneration).ToMutable(),
                Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<EBallPower>().UpgradeValueBy(3);
    }
}