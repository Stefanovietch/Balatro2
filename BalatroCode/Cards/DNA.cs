using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Cards;

public class DNA() : BalatroCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (PileType.Play.GetPile(this.Owner).Cards.Count > 1)
        {
            CardModel newCard = PileType.Play.GetPile(this.Owner).Cards.ElementAt(PileType.Play.GetPile(this.Owner).Cards.Count - 2);
            CardModel thisCard = PileType.Deck.GetPile(this.Owner).Cards.Single(c => c.Id == this.Id);
            await CardCmd.Transform(thisCard, newCard.CreateClone());
        }
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Retain);
    }
    
    protected override bool IsPlayable => CanPlay();

    protected override bool ShouldGlowGoldInternal => CanPlay();

    private new bool CanPlay()
    {
        return !PileType.Play.GetPile(this.Owner).IsEmpty;
    }
}
