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
        InvisibleJoker deckVersion;
        if (DeckVersion is InvisibleJoker invisibleJoker)
        {
            deckVersion = invisibleJoker;
        }
        else
        {
            var deckCard = this.Owner.Deck.Cards.FirstOrDefault(c => c is InvisibleJoker);
            if (deckCard is not InvisibleJoker invisibleJoker2) return;
            deckVersion = invisibleJoker2;
        }
        CountUp();
        deckVersion.CountUp();
        
        if (Counter >= 2)
        {
            var newCard =
                Owner.PlayerRng.Transformations.NextItem(PileType.Deck.GetPile(Owner).Cards.Where(c => c.Id != Id));
            if (newCard == null) return;
            newCard =  this.Owner.RunState.CloneCard(newCard);
            newCard.EnergyCost.SetCustomBaseCost(0);

            await CardCmd.Transform(deckVersion, newCard);
        }
    }

    public void CountUp()
    {
        Counter++;
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}