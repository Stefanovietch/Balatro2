using Balatro.BalatroCode.Extensions;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

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
        this.Card.AddKeyword(CardKeyword.Sly);
    }

    public override bool CanEnchant(CardModel card) => !card.CanonicalKeywords.Contains(CardKeyword.Sly);

    public override bool CanEnchantCardType(CardType cardType) => true;
}