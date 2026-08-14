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
        Owner.AddToMaxPotionCount(1);

        Owner.Potions.AddItem(new Duplicator());
        Owner.Potions.AddItem(new Duplicator());

        return Task.CompletedTask;
    }
}