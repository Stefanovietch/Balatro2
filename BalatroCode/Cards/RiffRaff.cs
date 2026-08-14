using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Cards;

public class RiffRaff() : BalatroCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        foreach (var card in CardFactory.GetForCombat(Owner,
                     Owner.Character.CardPool
                         .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                         .Where(c => c.Rarity == CardRarity.Common),
                     DynamicVars.Cards.IntValue, Owner.RunState.Rng.CombatCardGeneration))
        {
            card.EnergyCost.SetThisTurnOrUntilPlayed(0);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}