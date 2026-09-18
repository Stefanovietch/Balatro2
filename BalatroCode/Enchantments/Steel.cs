using Balatro.BalatroCode.Cards;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

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
        if (cardPlay?.Card.Owner != null && cardPlay.Card.CombatState != null)
            await StoneCard.CreateInHand(cardPlay.Card.Owner, 1, cardPlay.Card.CombatState);
    }

    public override bool CanEnchantCardType(CardType cardType)
    {
        return cardType is CardType.Attack or CardType.Skill;
    }
}