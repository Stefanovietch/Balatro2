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
        LowStakes source = this;
        if (source.Owner.Creature.CombatState == null) return;
        if (player != source.Owner || source.Owner.Creature.CombatState.RoundNumber > 1) return;
        List<CardModel> list = (await CardSelectCmd.FromHandForDiscard(choiceContext, source.Owner,
                new CardSelectorPrefs(source.SelectionScreenPrompt, 0, 999999999), null,
                source)).ToList<CardModel>();
        if (list.Count == 0) return;
        await CardCmd.DiscardAndDraw(choiceContext, (IEnumerable<CardModel>)list, list.Count);
    }

    public override Task AfterObtained()
    {
        LowStakes source = this;
        source.Owner.RelicGrabBag.Remove(ModelDb.Relic<GamblingChip>());
        return Task.CompletedTask;
    }
}