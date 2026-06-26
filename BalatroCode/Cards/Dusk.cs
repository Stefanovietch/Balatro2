using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class Dusk() : BalatroCard(3,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (!PileType.Hand.GetPile(this.Owner).IsEmpty) return;
        foreach (var cardPlayed in PileType.Play.GetPile(this.Owner).Cards.Where(c => c.Id != this.Id))
        {
            await CardCmd.AutoPlay(choiceContext, cardPlayed, null);
        }
    }

    protected override void OnUpgrade()
    {
        this.EnergyCost.UpgradeBy(-1);
    }

    protected override bool ShouldGlowGoldInternal => PileType.Hand.GetPile(this.Owner).Cards.Count <= 1;
}
