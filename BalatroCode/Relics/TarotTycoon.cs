using System.Runtime.InteropServices;
using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Relics;

public class TarotTycoon() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Common;


    public override async Task AfterPotionProcured(PotionModel potion)
    {
        await CreatureCmd.GainMaxHp(Owner.Creature, 1);
    }
}