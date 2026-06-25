using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Relics;

public class BigHands() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override bool TryModifyCardRewardOptions(
        Player player,
        List<CardCreationResult> options,
        CardCreationOptions creationOptions)
    {
        if (this.Owner != player) return false;
        if (options.Count < 1) return false;
        options.RemoveAt(0);
        return true;
    }
    
    public override Decimal ModifyHandDraw(Player player, Decimal count)
    {
        return player != this.Owner ? count : count + 2;
    }
}