using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Balatro.BalatroCode.Relics;

public class Observatory() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;

    private bool _usedThisCombat;
    
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return Task.CompletedTask;
        this._usedThisCombat = false;
        this.Status = RelicStatus.Active;
        return Task.CompletedTask;
    }
    
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (!card.IsUpgraded) return playCount;
        return this._usedThisCombat || card.Owner != this.Owner ? playCount : playCount + 1;
    }
    
    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        this._usedThisCombat = true;
        this.Flash();
        this.Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
    
    public override Task AfterCombatEnd(CombatRoom _)
    {
        this._usedThisCombat = false;
        this.Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}