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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Enchantments;

public class Lucky : CustomEnchantmentModel, IChance
{
    public override bool ShowAmount => false;
    
    protected override string CustomIconPath => "Balatro/images/enchantments/lucky.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("Chance", 5),
        new("ChanceMoney", 20),
        new DisplayVar<Lucky>("Numerator", e =>
        {
            var card = e.IsCanonical ? null : e.Card;
            if (card == null) return e.GetNumerator(null).ToString();
            var player = card.IsCanonical ? null : card.Owner;
            return e.GetNumerator(player).ToString();
        }),
    ];
    
    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (cardPlay?.Card.Owner is not { } player) return;
        if (this.RollChance(this.Card.Owner, DynamicVars["Chance"].IntValue))
            await PowerCmd.Apply<VigorPower>(choiceContext, player.Creature, Amount, player.Creature, null);

        if (this.RollChance(this.Card.Owner, DynamicVars["ChanceMoney"].IntValue))
            await PlayerCmd.GainGold(20, player);
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power is OopsAll6sPower) this.ModifyCard();
        return base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }

    public override bool CanEnchantCardType(CardType cardType) => cardType is CardType.Attack or CardType.Skill;
}