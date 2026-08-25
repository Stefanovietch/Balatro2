using Balatro.BalatroCode.Extensions;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Enchantments;

public class Wild : CustomEnchantmentModel
{
    public override bool ShowAmount => false;
    
    protected override string CustomIconPath => "Balatro/images/enchantments/wild.png";

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        var player = this.Card.Owner;
        List<CardPoolModel> list = player.UnlockState.CharacterCardPools.ToList();
        IEnumerable<CardModel> cards = list.SelectMany(pool => pool.AllCards);
        CardModel? card = CardFactory.GetDistinctForCombat(player, cards, 1, player.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (card == null) return;
        card.SetToFreeThisTurn();
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
    }

    public override bool CanEnchantCardType(CardType cardType) => true;
}