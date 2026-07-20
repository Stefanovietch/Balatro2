using Balatro.BalatroCode.Cards;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class ReservedParking() : BalatroCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self), IChance
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1),
        new DynamicVar("Chance", 2),
        new DisplayVar<ReservedParking>("Numerator", card => card
            .GetNumerator(card.IsCanonical ? null : card.Owner).ToString()),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var powers = PileType.Draw.GetPile(this.Owner).Cards.Where(c => c.Type == CardType.Power).TakeRandom(2, this.Owner.RunState.Rng.CombatCardSelection).ToList();
        if (powers.Count != 0)
        {
            foreach (var power in powers)
            {
                await CardPileCmd.Add(power, PileType.Hand);
            }
        }
        var powerCount = PileType.Hand.GetPile(this.Owner).Cards.Count(c => c.Type == CardType.Power);
        if (powerCount != 0)
        {
            if (this.RollChance(this.Owner, this.DynamicVars["Chance"].IntValue))
            {
                await PlayerCmd.GainGold(powerCount * 10, this.Owner);
            }
        }


    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Cards.UpgradeValueBy(1);
    }
}
