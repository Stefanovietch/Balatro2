using Balatro.BalatroCode.Cards;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class SpaceJoker() : BalatroCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self), IChance
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1),
        new DynamicVar("Chance", 4),
        new DisplayVar<SpaceJoker>("Numerator", card => card
            .GetNumerator(card.IsCanonical ? null : card.Owner).ToString()),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CardPileCmd.Draw(choiceContext, this.DynamicVars.Cards.BaseValue, this.Owner);
        var handUnupgraded = PileType.Hand.GetPile(this.Owner).Cards.Where(c => !c.IsUpgraded).ToList();
        if (handUnupgraded.Count == 0) return;
        foreach (var card in handUnupgraded)
        {
            if (this.RollChance(this.Owner, this.DynamicVars["Chance"].IntValue)) CardCmd.Upgrade(card);
        }
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Cards.UpgradeValueBy(1);
    }
}
