using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Cards;

public class MerryAndy() : BalatroCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(3),
        new CardsVar("Discard", 1),
        new PowerVar<DrawCardsNextTurnPower>(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CardPileCmd.Draw(choiceContext, this.DynamicVars.Cards.BaseValue, this.Owner);
        List<CardModel> cards = (await CardSelectCmd.FromHandForDiscard(choiceContext, this.Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, this.DynamicVars["Discard"].IntValue), null, this)).ToList();
        await CardCmd.Discard(choiceContext, cards);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, this.Owner.Creature, -this.DynamicVars["DrawCardsNextTurnPower"].BaseValue, this.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Cards.UpgradeValueBy(1);
        this.DynamicVars["Discard"].UpgradeValueBy(1);
    }
}
