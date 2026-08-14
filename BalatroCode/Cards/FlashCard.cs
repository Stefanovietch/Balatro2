using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Cards;

public class FlashCard() : BalatroCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, DynamicVars.Cards.IntValue);
        var drawPile = PileType.Draw.GetPile(Owner);
        var firstX = drawPile.Cards.Take(DynamicVars.Cards.IntValue).ToHashSet();
        var cardsToDiscard =
            (await CardSelectCmd.FromCombatPile(choiceContext, drawPile, Owner, prefs, firstX.Contains)).ToList();
        if (cardsToDiscard.Count == 0) await CardPileCmd.Draw(choiceContext, Owner);
        else await CardCmd.DiscardAndDraw(choiceContext, cardsToDiscard, 1);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2);
    }
}