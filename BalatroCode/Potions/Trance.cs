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

public class Trance : BalatroPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    
    public override string CustomPackedImagePath => "/potions/trance.png".ImagePath();

    public override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        ..HoverTipFactory.FromEnchantment<BlueSeal>()
    ];
    
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        CardModel? card = (await CardSelectCmd.FromHand(choiceContext, this.Owner, prefs, c => ModelDb.Enchantment<BlueSeal>().CanEnchant(c), this)).FirstOrDefault();
        if (card == null) return;
        CardCmd.Enchant<BlueSeal>(card, 1);
        CardCmd.Upgrade(card);
        if (card.DeckVersion is not { } deckVersion) return;
        CardCmd.Enchant<BlueSeal>(deckVersion, 1);
        CardCmd.Upgrade(deckVersion);
    }
}