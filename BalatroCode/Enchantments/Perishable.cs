using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Rooms;

namespace Balatro.BalatroCode.Enchantments;

public class Perishable : CustomEnchantmentModel
{
    public override bool ShowAmount => true;

    protected override string CustomIconPath => "Balatro/images/enchantments/perishable.png";

    public override async Task AfterCombatEnd(CombatRoom combatRoom)
    {
        Amount -= 1;
        if (Amount <= 0)
        {
            await CardPileCmd.RemoveFromCombat(Card);
            if (Card.DeckVersion != null) await CardPileCmd.RemoveFromDeck(Card.DeckVersion);
        }

        ModifyCard();
    }

    public override bool CanEnchantCardType(CardType cardType)
    {
        return true;
    }
}