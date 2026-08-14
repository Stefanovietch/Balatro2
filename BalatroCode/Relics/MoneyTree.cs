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
        return IsBeforeAct3TreasureChest(runState);
    }

    public override Task AfterObtained()
    {
        if (Owner.Character is not Character.Balatro balatro) return Task.CompletedTask;
        Character.Balatro.MaxCombatGold.Set(Owner, 400);
        return Task.CompletedTask;
    }
}