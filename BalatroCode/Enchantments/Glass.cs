using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Extensions;
using Balatro.BalatroCode.Powers;
using BaseLib.Abstracts;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Enchantments;

public class Glass : CustomEnchantmentModel, IChance
{
    public override bool ShowAmount => false;
    
    public override bool HasExtraCardText => true;
    
    protected override string CustomIconPath => "Balatro/images/enchantments/glass.png";
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("Chance", 4),
        new DisplayVar<Glass>("Numerator", e =>         {
            var card = e.IsCanonical ? null : e.Card;
            if (card == null) return e.GetNumerator(null).ToString();
            var player = card.IsCanonical ? null : card.Owner;
            return e.GetNumerator(player).ToString();
        }),
    ];

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (this.RollChance(this.Card.Owner, DynamicVars["Chance"].IntValue))
            await CardCmd.Exhaust(choiceContext, this.Card);
    }

    public override Decimal EnchantDamageMultiplicative(Decimal originalDamage, ValueProp props)
    {
        return !props.IsPoweredAttack() ? 1M : 2M;
    }
    
    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power is OopsAll6sPower) this.ModifyCard();
        return base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }
    
    public override bool CanEnchantCardType(CardType cardType) => cardType == CardType.Attack;
}