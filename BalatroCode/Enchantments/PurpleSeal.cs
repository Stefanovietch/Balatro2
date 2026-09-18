using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Enchantments;

public class PurpleSeal : CustomEnchantmentModel
{
    public override bool ShowAmount => false;

    protected override string CustomIconPath => "Balatro/images/enchantments/purple_seal.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Sly)
    ];

    protected override void OnEnchant()
    {
        Card.AddKeyword(CardKeyword.Sly);
    }

    public override bool CanEnchant(CardModel card)
    {
        return !card.CanonicalKeywords.Contains(CardKeyword.Sly);
    }

    public override bool CanEnchantCardType(CardType cardType)
    {
        return true;
    }
}