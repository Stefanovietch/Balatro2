using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Cards;

public class SteelJoker() : BalatroCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0),
        new CalculationExtraVar(1),
        new CalculatedVar("PlatingPower").WithMultiplier((card, _) => PileType.Deck.GetPile(card.Owner).Cards.Count(c => c.IsUpgraded))
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<PlatingPower>(choiceContext, this.Owner.Creature, (int)((CalculatedVar)base.DynamicVars["PlatingPower"]).Calculate(play.Target), this.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Innate);
    }
}
