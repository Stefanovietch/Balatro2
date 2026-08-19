using System.ComponentModel.Design;
using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Balatro.BalatroCode.Relics;

public class LowStakes() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (this.Owner.Creature.CombatState == null) return;
        if (player != this.Owner || this.Owner.Creature.CombatState.RoundNumber > 1) return;
        var list = (await CardSelectCmd.FromHandForDiscard(choiceContext, this.Owner,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 0, 10), null, this)).ToList();
        if (list.Count == 0) return;
        await CardCmd.DiscardAndDraw(choiceContext, list, list.Count);
    }

    public override Task AfterObtained()
    {
        this.Owner.RelicGrabBag.Remove(ModelDb.Relic<GamblingChip>());
        return Task.CompletedTask;
    }
}