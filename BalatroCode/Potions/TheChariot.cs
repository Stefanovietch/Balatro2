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

public class TheChariot : BalatroPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    
    public override string CustomPackedImagePath => "/potions/the_chariot.png".ImagePath();

    public override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        ..HoverTipFactory.FromEnchantment<Steel>()
    ];
    
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        CardModel? card = (await CardSelectCmd.FromHand(choiceContext, this.Owner, prefs, c => ModelDb.Enchantment<Steel>().CanEnchant(c), this)).FirstOrDefault();
        if (card == null) return;
        CardCmd.Enchant<Steel>(card, 1);
        if (card.DeckVersion is not { } deckVersion) return;
        CardCmd.Enchant<Steel>(deckVersion, 1);
    }
}