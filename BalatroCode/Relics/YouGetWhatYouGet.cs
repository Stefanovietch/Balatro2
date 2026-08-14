using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace Balatro.BalatroCode.Relics;

public class YouGetWhatYouGet() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (Owner != player || room is not { RoomType: RoomType.Monster or RoomType.Elite or RoomType.Boss })
            return base.TryModifyRewards(player, rewards, room);
        foreach (var reward in rewards.ToList().OfType<GoldReward>()) rewards.Remove(reward);
        return base.TryModifyRewards(player, rewards, room);
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player || Owner.PlayerCombatState == null) return;
        decimal goldAmount = Owner.PlayerCombatState.Hand.Cards.Count * 2;
        await PlayerCmd.GainGold(goldAmount, Owner);
    }
}