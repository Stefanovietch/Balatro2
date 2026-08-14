using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Powers;
using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class BusinessCard() : BalatroCard(1,
    CardType.Power, CardRarity.Common,
    TargetType.Self), IChance
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("Chance", 4),
        new DisplayVar<BusinessCard>("Numerator", card => card
            .GetNumerator(card.IsCanonical ? null : card.Owner).ToString()),
        new PowerVar<BusinessCardPower>(20)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<BusinessCardPower>(choiceContext, Owner.Creature,
            DynamicVars.Power<BusinessCardPower>().BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<BusinessCardPower>().UpgradeValueBy(5);
    }
}