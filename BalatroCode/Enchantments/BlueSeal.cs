using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Balatro.BalatroCode.Enchantments;

public class BlueSeal : CustomEnchantmentModel
{
    public override bool ShowAmount => false;

    protected override string CustomIconPath => "Balatro/images/enchantments/blue_seal.png";

    public override bool CanEnchantCardType(CardType cardType)
    {
        return true;
    }
}