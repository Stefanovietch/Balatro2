using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Cards;

public class Seance() : BalatroCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (!OnlySkills()) return;
        foreach (CardModel card in CardFactory.GetForCombat(this.Owner, this.Owner.Character.CardPool.GetUnlockedCards(this.Owner.UnlockState, this.Owner.RunState.CardMultiplayerConstraint).Where<CardModel>((Func<CardModel, bool>) (c => c.Type == CardType.Skill)), this.DynamicVars.Cards.IntValue, this.Owner.RunState.Rng.CombatCardGeneration))
        {
            card.SetToFreeThisCombat();
            if (this.IsUpgraded) CardCmd.Upgrade(card);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, this.Owner, CardPilePosition.Random));
        }
    }

    protected override bool ShouldGlowGoldInternal => OnlySkills();

    protected override void OnUpgrade()
    {

    }

    private bool OnlySkills()
    {
        var amountNonSkills = PileType.Hand.GetPile(this.Owner).Cards.Count(c => !c.Equals(this) && c.Type != CardType.Skill);
        return amountNonSkills == 0;
    }
}
