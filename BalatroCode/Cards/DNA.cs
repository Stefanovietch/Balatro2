using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
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
        if (!CanPlay()) return;
        var newCard = CombatManager.Instance.History.CardPlaysFinished.LastOrDefault(c => c.CardPlay.Card is not DNA)?.CardPlay.Card;
        if (newCard == null) return;
        
        await CardPileCmd.AddGeneratedCardToCombat(newCard.CreateClone(), PileType.Discard, this.Owner);

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
        await CardPileCmd.Add(this, PileType.Exhaust);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == this) await CardPileCmd.RemoveFromCombat(this, true);
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