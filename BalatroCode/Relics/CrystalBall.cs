using Balatro.BalatroCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Rewards;

namespace Balatro.BalatroCode.Relics;

public class CrystalBall() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;
    
    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPotion<Duplicator>()
    ];

    public override async Task AfterObtained()
    {
        await PlayerCmd.GainMaxPotionCount(1, this.Owner);
        _ = TaskHelper.RunSafely(ObtainPotions());
    }

    private async Task ObtainPotions()
    {
        await PotionCmd.TryToProcure(ModelDb.Potion<Duplicator>().ToMutable(), this.Owner);
        await PotionCmd.TryToProcure(ModelDb.Potion<Duplicator>().ToMutable(), this.Owner);
    }
}