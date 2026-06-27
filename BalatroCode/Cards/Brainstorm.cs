using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class Brainstorm() : BalatroCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (this.Owner.PlayerCombatState?.PlayPile.Cards.Count > 1)
        {
            var card = CombatManager.Instance.History.CardPlaysFinished.First(c => c.HappenedThisTurn(play.Card.CombatState)).CardPlay.Card;
            await CardCmd.AutoPlay(choiceContext, card, null);
        }
    }

    protected override void OnUpgrade()
    {
        this.EnergyCost.UpgradeBy(-1);
    }
    
    protected override bool IsPlayable => CanPlay();

    protected override bool ShouldGlowGoldInternal => CanPlay();

    private new bool CanPlay()
    {
        return !this.Owner.PlayerCombatState?.PlayPile.IsEmpty ?? false;
    }
}
