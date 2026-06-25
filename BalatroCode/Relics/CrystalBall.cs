using Balatro.BalatroCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Rewards;

namespace Balatro.BalatroCode.Relics;

public class CrystalBall() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override Task AfterObtained()
    {
        this.Owner.AddToMaxPotionCount(1);

        this.Owner.Potions.AddItem(new Duplicator());
        this.Owner.Potions.AddItem(new Duplicator());
        
        return Task.CompletedTask;
    }
}