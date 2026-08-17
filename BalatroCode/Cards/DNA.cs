using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Balatro.BalatroCode.Cards;

public class DNA() : BalatroCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override bool CanBeGeneratedInCombat => false;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (CanPlay())
        {
            var newCard = CombatManager.Instance.History.CardPlaysFinished.LastOrDefault(c => c.HappenedThisTurn(play.Card.CombatState))?.CardPlay.Card;
            
            if (newCard == null) return;
            var newCombatCard = await CardCmd.Transform(this, newCard);
            if (newCombatCard != null) await CardPileCmd.Add(newCombatCard.Value.cardAdded, PileType.Discard);

            CardModel deckVersion;
            if (DeckVersion is DNA)
            {
                deckVersion = DeckVersion;
            }
            else
            {
                var deckCard = this.Owner.Deck.Cards.FirstOrDefault(c => c is DNA);
                if (deckCard is not DNA) return;
                deckVersion = deckCard;
            }
            var cardClone = this.Owner.RunState.CloneCard(newCard);

            await CardCmd.Transform(deckVersion, cardClone, CardPreviewStyle.None);
            
            await CardPileCmd.RemoveFromCombat(this);

        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }

    protected override bool IsPlayable => CanPlay();

    protected override bool ShouldGlowGoldInternal => CanPlay();

    private new bool CanPlay()
    {
        return CombatManager.Instance.History.CardPlaysFinished.Any();
    }
}