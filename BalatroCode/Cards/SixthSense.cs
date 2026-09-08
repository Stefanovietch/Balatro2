using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Cards;

public class SixthSense() : BalatroCard(-1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    private bool _cardPlayed = false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable, CardKeyword.Innate];

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Ethereal);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (_cardPlayed || this.Pile?.Type != PileType.Hand || Owner != cardPlay.Card.Owner) return;
        var card = cardPlay.Card;
        if (card.Pile != null && card.Pile.Type != PileType.None)
            await CardPileCmd.RemoveFromCombat(card);
        if (card.DeckVersion != null && card.DeckVersion.Pile != null)
            await CardPileCmd.RemoveFromDeck(card.DeckVersion);
        if (Owner.RunState.CurrentRoom is CombatRoom room)
            room.AddExtraReward(Owner,
                new CardReward(
                    new CardCreationOptions(ModelDb.CardPool<ColorlessCardPool>().AllCards,
                        CardCreationSource.Encounter, CardRarityOddsType.Uniform), 3, Owner));

        _cardPlayed = true;
    }
}