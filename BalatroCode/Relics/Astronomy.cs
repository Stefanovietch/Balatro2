using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Balatro.BalatroCode.Relics;

public class Astronomy() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override Task AfterObtained()
    {
        this.Owner.SubtractFromMaxPotionCount(1);
        this.Owner.RelicGrabBag.Remove(ModelDb.Relic<RazorTooth>());
        return Task.CompletedTask;
    }
  
    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != this.Owner || !cardPlay.Card.IsUpgradable) 
            return Task.CompletedTask;
        CardCmd.Upgrade(cardPlay.Card);
        return Task.CompletedTask;
    }
}