using Balatro.BalatroCode.Extensions;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Balatro.BalatroCode.Enchantments;

public class Perishable : CustomEnchantmentModel
{
    public override bool ShowAmount => true;
    
    protected override string CustomIconPath => "Balatro/images/enchantments/perishable.png";
    
    public override async Task AfterCombatEnd(CombatRoom combatRoom)
    {
        this.Amount -= 1;
        if (this.Amount <= 0)
        {
            await CardPileCmd.RemoveFromCombat(this.Card);
            if (this.Card.DeckVersion != null) await CardPileCmd.RemoveFromDeck(this.Card.DeckVersion);
        }
        ModifyCard(); }
    
    public override bool CanEnchantCardType(CardType cardType) => true;
}