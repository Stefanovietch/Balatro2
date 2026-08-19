using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rooms;

namespace Balatro.BalatroCode.Relics;

public class ROI() : BalatroRelic
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