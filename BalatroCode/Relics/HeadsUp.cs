using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Relics;

public class HeadsUp() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;
    
    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != this.Owner ? amount : amount + 1;
    }
}