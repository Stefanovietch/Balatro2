using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Balatro.BalatroCode.Cards;

public class SmearedJoker() : BalatroCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        foreach (var card in PileType.Hand.GetPile(Owner).Cards.Where(c => c.EnergyCost.Canonical >= 0))
        {
            var cost = Owner.RunState.Rng.CombatEnergyCosts.NextInt(4);
            card.EnergyCost.SetThisCombat(cost);
            NCard.FindOnTable(card)?.PlayRandomizeCostAnim();
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1);
    }
}