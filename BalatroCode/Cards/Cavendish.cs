using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Powers;
using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class Cavendish() : BalatroCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self), IChance
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Chance", 1000),
        new DisplayVar<BusinessCard>("Numerator", card => card.GetNumerator(card.Owner).ToString())
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<CavendishPower>(choiceContext, this.Owner.Creature, 3, this.Owner.Creature, this);
        if (this.RollChance(this.Owner, this.DynamicVars["Chance"].IntValue))
        {
            await CardPileCmd.RemoveFromCombat(this);
            if (this.DeckVersion is not Cavendish deckVersion)
                return;
            await CardPileCmd.RemoveFromDeck(deckVersion);
        }
    }

    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Ethereal);
    }
}
