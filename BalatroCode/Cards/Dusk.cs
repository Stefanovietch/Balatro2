using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Combat;
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
        if (!PileType.Hand.GetPile(Owner).IsEmpty) return;
        foreach (var cardPlayed in CombatManager.Instance.History.CardPlaysFinished.Where(c =>
                     !c.CardPlay.Card.Equals(this) && c.HappenedThisTurn(play.Card.CombatState) &&
                     c.CardPlay.Card.Owner == play.Card.Owner))
            await CardCmd.AutoPlay(choiceContext, cardPlayed.CardPlay.Card, null);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    protected override bool ShouldGlowGoldInternal => PileType.Hand.GetPile(Owner).Cards.Count <= 1;
}