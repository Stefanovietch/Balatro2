using Balatro.BalatroCode.Extensions;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Balatro.BalatroCode.Enchantments;

public class BlueSeal : CustomEnchantmentModel
{
    public override bool ShowAmount => false;
    
    protected override string CustomIconPath => "Balatro/images/enchantments/blue_seal.png";
    
    public override bool CanEnchantCardType(CardType cardType) => true;
}