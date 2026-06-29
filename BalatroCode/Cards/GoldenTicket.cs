using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Cards;

public class GoldenTicket() : BalatroCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new GoldVar(4)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var card = CardFactory.GetForCombat(this.Owner, 
            this.Owner.Character.CardPool.GetUnlockedCards(this.Owner.UnlockState, this.Owner.RunState.CardMultiplayerConstraint)
                .Where(c => c.Rarity == CardRarity.Rare), 
            1, this.Owner.RunState.Rng.CombatCardGeneration).First();
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, this.Owner);
        
        var amountRareCards = PileType.Deck.GetPile(this.Owner).Cards.Count(c => c.Rarity == CardRarity.Rare);
        await PlayerCmd.GainGold(amountRareCards * this.DynamicVars.Gold.BaseValue, this.Owner);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Gold.UpgradeValueBy(2);
    }
}
