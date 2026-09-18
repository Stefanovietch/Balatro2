using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class Superposition() : BalatroCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override bool ShouldGlowGoldInternal => HaveStraight();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (HaveStraight())
            await PotionCmd.TryToProcure(
                PotionFactory.CreateRandomPotionInCombat(Owner, Owner.RunState.Rng.CombatPotionGeneration).ToMutable(),
                Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    private bool HaveStraight()
    {
        var amountList = PileType.Hand.GetPile(Owner).Cards.Where(c => !c.Equals(this))
            .Select(c => c.EnergyCost.GetAmountToSpend()).ToList();
        return amountList.Contains(0) && amountList.Contains(1) && amountList.Contains(2) && amountList.Contains(3);
    }
}