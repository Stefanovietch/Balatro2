using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Extensions;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Balatro.BalatroCode.Enchantments;

public class Steel : CustomEnchantmentModel
{
    public override bool ShowAmount => false;
    
    protected override string CustomIconPath => "Balatro/images/enchantments/steel.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<StoneCard>()
    ];
    
    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (cardPlay?.Card.Owner != null && cardPlay.Card.CombatState != null) await StoneCard.CreateInHand(cardPlay.Card.Owner, 1, cardPlay.Card.CombatState);

    }

    public override bool CanEnchantCardType(CardType cardType) => cardType is CardType.Attack or CardType.Skill;
}