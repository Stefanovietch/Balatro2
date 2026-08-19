using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Balatro.BalatroCode.Relics;

public class NestEgg() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override async Task AfterObtained()
    {
        await PlayerCmd.GainGold(300, this.Owner);
    }
}