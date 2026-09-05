using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Patches;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class Brainstorm() : BalatroCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.ReplayStatic)
    ];
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var cardPlayEntry = CombatManager.Instance.History.CardPlaysFinished
            .FirstOrDefault(c => c.HappenedThisTurn(play.Card.CombatState) && c.CardPlay.Card is not Brainstorm);
        if (cardPlayEntry == null) return;
        PowerReplayPatch.IsReplaying = true;
        try
        {
            await CardCmd.AutoPlay(choiceContext, cardPlayEntry.CardPlay.Card, cardPlayEntry.CardPlay.Target);
        }
        finally
        {
            PowerReplayPatch.IsReplaying = false;
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    protected override bool IsPlayable => CanPlay();

    protected override bool ShouldGlowGoldInternal => CanPlay();

    private new bool CanPlay()
    {
        var card = CombatManager.Instance.History.CardPlaysFinished.FirstOrDefault(c => c.HappenedThisTurn(this.CombatState));
        return card is { CardPlay.Card: not (DNA or InvisibleJoker) };
    }
}