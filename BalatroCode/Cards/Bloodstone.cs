using Balatro.BalatroCode.Cards;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class Bloodstone() : BalatroCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.RandomEnemy), IChance
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Chance", 2),
        new DisplayVar<Bloodstone>("Numerator", card => card
            .GetNumerator(card.IsCanonical ? null : card.Owner).ToString())
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var attacksThisTurn = CombatManager.Instance.History.CardPlaysFinished
            .Where(c => c.CardPlay.Card.Owner == this.Owner && c.HappenedThisTurn(this.CombatState) &&
                        c.CardPlay.Card.Type == CardType.Attack)
            .Select(c => c.CardPlay.Card)
            .ToList();

        foreach (var card in attacksThisTurn)        
        {
            if (this.RollChance(this.Owner, this.DynamicVars["Chance"].IntValue))
            {
                await CardCmd.AutoPlay(choiceContext, card, null);
            }
        }
    }

    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Exhaust);
    }
}
