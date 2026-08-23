using Balatro.BalatroCode.Cards;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class TheIdol() : BalatroCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self), IRandomType
{
    private int _cost = -1;

    public int CurrentCost
    {
        get => _cost;
        set
        {
            AssertMutable();
            _cost = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DisplayVar<TheIdol>("Type", card => ((IRandomType)card).GetTypeString()),
        new DisplayVar<TheIdol>("Cost", card => card.GetCostString())
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var card = PileType.Draw.GetPile(Owner).Cards
            .Where(c => c.Type == CurrentType && c.EnergyCost.GetAmountToSpend() == CurrentCost)
            .TakeRandom(1, Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();
        if (card == null) return;
        card.EnergyCost.SetThisTurnOrUntilPlayed(0);
        await CardPileCmd.Add(card, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }

    public void SetRandomType()
    {
        if (!IsInCombat)
        {
            CurrentType = CardType.None;
            return;
        }
        var card = PileType.Draw.GetPile(Owner).Cards
            .Where(c => c.Type is CardType.Attack or CardType.Skill or CardType.Power &&
                        c.EnergyCost.GetAmountToSpend() is >= 1 and <= 3)
            .TakeRandom(1, Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();
        if (card == null)
        {
            CurrentCost = -1;
            CurrentType = CardType.None;
        }
        else
        {
            CurrentCost = card.EnergyCost.GetAmountToSpend();
            CurrentType = card.Type;
        }
    }

    public CardType CurrentType { get; set; } = CardType.None;

    private string GetCostString()
    {
        return CurrentCost == -1 ? "(1|2|3)" : CurrentCost.ToString();
    }
}