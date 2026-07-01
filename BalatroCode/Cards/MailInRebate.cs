using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Cards;

public class MailInRebate() : BalatroCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self), IRandomType
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2),
        new GoldVar(10)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CardModel? card = (await CardSelectCmd.FromHandForDiscard(choiceContext, this.Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt,1), null, this)).FirstOrDefault();
        if (card != null)
        {
            await CardCmd.Discard(choiceContext, card);
            if (card.Type == CurrentType) await PlayerCmd.GainGold(this.DynamicVars.Gold.BaseValue, this.Owner);
        }
        await CardPileCmd.Draw(choiceContext, this.DynamicVars.Cards.BaseValue, this.Owner);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Cards.UpgradeValueBy(1);
    }

    public CardType CurrentType { get; set; } = CardType.None;
}
