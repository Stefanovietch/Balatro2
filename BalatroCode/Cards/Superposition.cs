using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.PotionPools;

namespace Balatro.BalatroCode.Cards;

public class Superposition() : BalatroCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CardPileCmd.Draw(choiceContext, this.DynamicVars.Cards.BaseValue, this.Owner);
        if (HaveStraight()) await PotionCmd.TryToProcure(PotionFactory.CreateRandomPotionInCombat(this.Owner, this.Owner.RunState.Rng.CombatPotionGeneration, this.Owner.Character.PotionPool.GetUnlockedPotions(this.Owner.UnlockState).Concat(ModelDb.PotionPool<SharedPotionPool>().GetUnlockedPotions(this.Owner.UnlockState))).ToMutable(), this.Owner);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Cards.UpgradeValueBy(1);
    }
    protected override bool ShouldGlowGoldInternal => HaveStraight();

    private bool HaveStraight()
    {
        var amountList = PileType.Hand.GetPile(this.Owner).Cards.Where(c => !c.Equals(this))
            .Select(c => c.EnergyCost.GetAmountToSpend()).ToList();
        return amountList.Contains(0) && amountList.Contains(1) && amountList.Contains(2) && amountList.Contains(3);
    }
}
