using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Relics;

public class HeadsUp() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(1)
    ];
    
    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != this.Owner ? amount : amount + this.DynamicVars.Energy.BaseValue;
    }
}