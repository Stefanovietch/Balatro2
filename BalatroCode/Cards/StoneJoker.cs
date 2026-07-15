using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class StoneJoker() : BalatroCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(5, ValueProp.Move),
        new CalculationBaseVar(0),
        new CalculationExtraVar(1),
        new CalculatedVar("StoneCards").WithMultiplier((card, _) => PileType.Draw.GetPile(card.Owner).Cards.Count(c => c is StoneCard)
                                                                     + PileType.Hand.GetPile(card.Owner).Cards.Count(c => c is StoneCard)
                                                                     + PileType.Discard.GetPile(card.Owner).Cards.Count(c => c is StoneCard)
                                                                     + PileType.Exhaust.GetPile(card.Owner).Cards.Count(c => c is StoneCard))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(this.CombatState);
        await StoneCard.CreateInHand(this.Owner, 1, this.CombatState);
        for (var _ = 0; _ < (int)((CalculatedVar)base.DynamicVars["PlatingPower"]).Calculate(null); _++) await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.Block, play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(1);
    }
}
