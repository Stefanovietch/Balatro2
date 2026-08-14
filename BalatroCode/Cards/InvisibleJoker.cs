using Balatro.BalatroCode.Cards;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Balatro.BalatroCode.Cards;

public class InvisibleJoker() : BalatroCard(3,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    private int _counter;

    [SavedProperty]
    public int Counter
    {
        get => _counter;
        set
        {
            AssertMutable();
            _counter = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override bool CanBeGeneratedInCombat => false;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Counter++;
        if (Counter >= 2)
        {
            var newCard =
                Owner.PlayerRng.Transformations.NextItem(PileType.Deck.GetPile(Owner).Cards.Where(c => c.Id != Id));
            if (newCard == null) return;
            newCard = newCard.CreateClone();
            newCard.EnergyCost.SetCustomBaseCost(0);
            var thisCard = PileType.Deck.GetPile(Owner).Cards.Single(c => c.Id == Id);
            await CardCmd.Transform(thisCard, newCard);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}