using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace Balatro.BalatroCode.Relics;

public class Liquidation() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (Owner.Creature.IsDead || !(room is MerchantRoom))
            return;
        Flash();
        await PlayerCmd.GainGold(150, Owner);
    }
}