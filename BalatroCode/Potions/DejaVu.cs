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

public class DejaVu : BalatroPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    
    public override string CustomPackedImagePath => "/potions/deja_vu.png".ImagePath();

    public override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        ..HoverTipFactory.FromEnchantment<Glam>()
    ];
    
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        CardModel? card = (await CardSelectCmd.FromHand(choiceContext, this.Owner, prefs, c => ModelDb.Enchantment<Glam>().CanEnchant(c), this)).FirstOrDefault();
        if (card == null) return;
        CardCmd.Enchant<Glam>(card, 1);
        if (card.DeckVersion is not { } deckVersion) return;
        CardCmd.Enchant<Glam>(deckVersion, 1);
    }
}