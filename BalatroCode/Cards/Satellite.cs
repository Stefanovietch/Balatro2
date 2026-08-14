using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class Satellite() : BalatroCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("GoldPerCard", 2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var cardToUpgrade = PileType.Deck.GetPile(Owner).Cards.Where(c => !c.IsUpgraded)
            .TakeRandom(1, Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();
        if (cardToUpgrade != null)
            CardCmd.Upgrade(cardToUpgrade);
        await PlayerCmd.GainGold(
            PileType.Deck.GetPile(Owner).Cards.Count(c => c.IsUpgraded) * DynamicVars["GoldPerCard"].BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["GoldPerCard"].UpgradeValueBy(1);
    }
}