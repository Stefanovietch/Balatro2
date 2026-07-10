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
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("GoldPerCard", 2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var cardToUpgrade = PileType.Deck.GetPile(this.Owner).Cards.Where(c => !c.IsUpgraded).TakeRandom(1, this.Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();
        if (cardToUpgrade != null)
            CardCmd.Upgrade(cardToUpgrade);
        await PlayerCmd.GainGold(PileType.Deck.GetPile(this.Owner).Cards.Count(c => c.IsUpgraded) * this.DynamicVars["GoldPerCard"].BaseValue, this.Owner);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars["GoldPerCard"].UpgradeValueBy(1);
    }
}
