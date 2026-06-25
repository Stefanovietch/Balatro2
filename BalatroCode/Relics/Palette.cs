using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Balatro.BalatroCode.Relics;

public class Palette() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Event;

    
    public override Decimal ModifyHandDraw(Player player, Decimal count)
    {
        return player != this.Owner ? count : count + 1;
    }
}