using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Cards;

public class HitTheRoad() : BalatroCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0),
        new CalculationExtraVar(1),
        new CalculatedVar("StrengthPower").WithMultiplier((c, _) =>
        {
            if (c.Owner.PlayerCombatState == null) return 0;
            if (c.Owner.Character is not Character.Balatro balatro) return 0;
            return balatro.CardsDiscardedThisTurn.Get(c.Owner.PlayerCombatState);
        })
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<StrengthPower>(choiceContext, this.Owner.Creature, 
            (int)((CalculatedVar)base.DynamicVars["StrengthPower"]).Calculate(play.Target), 
            this.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Exhaust);
    }
}
