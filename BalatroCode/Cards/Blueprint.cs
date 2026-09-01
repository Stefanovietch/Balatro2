using System.Reflection;
using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Patches;
using BaseLib.Commands;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Cards;

public class Blueprint() : BalatroCard(1,
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
            .LastOrDefault(c => c.HappenedThisTurn(this.CombatState) && c.CardPlay.Card is not Blueprint);
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
        return CombatManager.Instance.History.CardPlaysFinished.LastOrDefault(c => c.HappenedThisTurn(this.CombatState) && c.CardPlay.Card is not (Blueprint or Brainstorm)) != null;
    }
}