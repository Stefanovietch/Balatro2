using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace Balatro.BalatroCode.Relics;

public class ROI : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not MerchantRoom merchantRoom || merchantRoom.GetLocalInventory().Player != Owner)
        {
            Status = RelicStatus.Normal;
            return Task.CompletedTask;
        }

        Flash();
        Status = RelicStatus.Active;
        return Task.CompletedTask;
    }
}