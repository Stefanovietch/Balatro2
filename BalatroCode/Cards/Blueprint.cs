using System.Reflection;
using Balatro.BalatroCode.Cards;
using BaseLib.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class Blueprint() : BalatroCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var card = CombatManager.Instance.History.CardPlaysFinished
            .LastOrDefault(c => c.HappenedThisTurn(this.CombatState) && c.CardPlay.Card is not Blueprint)?.CardPlay.Card;
        if (card == null) return;
        var model = choiceContext.LastInvolvedModel;
        if (model is null) return;
        choiceContext.PopModel(model);
        await CardCmd.AutoPlay(choiceContext, card, null);
        choiceContext.PushModel(model);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    protected override bool IsPlayable => CanPlay();

    protected override bool ShouldGlowGoldInternal => CanPlay();

    private new bool CanPlay()
    {
        return CombatManager.Instance.History.CardPlaysFinished.LastOrDefault(c => c.HappenedThisTurn(this.CombatState) && c.CardPlay.Card is not (Blueprint or Brainstorm)) != null;
    }
}