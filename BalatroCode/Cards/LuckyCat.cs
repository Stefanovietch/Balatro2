using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Powers;
using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class LuckyCat() : BalatroCard(2,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self), IChance
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("Chance", 4),
        new DisplayVar<LuckyCat>("Numerator", card => card
            .GetNumerator(card.IsCanonical ? null : card.Owner).ToString()),
        new PowerVar<LuckyCatPower>(10)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<LuckyCatPower>(choiceContext, Owner.Creature, DynamicVars.Power<LuckyCatPower>().BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}