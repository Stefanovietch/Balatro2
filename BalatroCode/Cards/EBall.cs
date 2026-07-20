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
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Chance", 4),
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
        await PowerCmd.Apply<EBallPower>(choiceContext, play.Target, this.DynamicVars.Power<EBallPower>().BaseValue, this.Owner.Creature, this);
        if (this.RollChance(this.Owner, this.DynamicVars["Chance"].IntValue))
        {
            await PotionCmd.TryToProcure(PotionFactory.CreateRandomPotionInCombat(this.Owner, this.Owner.RunState.Rng.CombatPotionGeneration, this.Owner.Character.PotionPool.GetUnlockedPotions(this.Owner.UnlockState).Concat(ModelDb.PotionPool<SharedPotionPool>().GetUnlockedPotions(this.Owner.UnlockState))).ToMutable(), this.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Power<EBallPower>().UpgradeValueBy(3);
    }
}
