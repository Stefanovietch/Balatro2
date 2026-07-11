using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Relics;

public class MoneyTree() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Shop;
    
    public override bool IsAllowed(IRunState runState)
    {
        return RelicModel.IsBeforeAct3TreasureChest(runState);
    }

    public override Task AfterObtained()
    {
        if (this.Owner.Character is not Character.Balatro balatro) return Task.CompletedTask;
        balatro.MaxCombatGold.Set(this.Owner, 400);
        return Task.CompletedTask;
    }
}