using Balatro.BalatroCode.Enchantments;
using Balatro.BalatroCode.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace Balatro.BalatroCode.Potions;

public class TheMagician : BalatroPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    
    public override string CustomPackedImagePath => "/potions/the_magician.png".ImagePath();

    public override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        ..HoverTipFactory.FromEnchantment<Lucky>()
    ];
    
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 2);
        List<CardModel> cards = (await CardSelectCmd.FromHand(choiceContext, this.Owner, prefs, c => ModelDb.Enchantment<Lucky>().CanEnchant(c), this)).ToList();
        if (cards.Count == 0) return;
        foreach (var card in cards)
        {
            CardCmd.Enchant<Lucky>(card, 6);
            if (card.DeckVersion is not { } deckVersion) return;
            CardCmd.Enchant<Lucky>(deckVersion, 6);
        }

    }
}