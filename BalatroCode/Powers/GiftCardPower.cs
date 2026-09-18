using System.Collections;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Powers;

public class GiftCardPower : BalatroPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (Owner.Player == null) return base.AfterCombatEnd(room);
        var rewardsSetSynchronizer = Traverse.Create(RunManager.Instance).Property("RewardsSetSynchronizer")
            .GetValue<RewardsSetSynchronizer>();
        var playerRewardState = Traverse.Create(rewardsSetSynchronizer).Method("GetRewardStateForPlayer", Owner.Player)
            .GetValue();
        var rewardsStack = Traverse.Create(playerRewardState).Field("rewardsStack").GetValue<IList>();
        _ = TaskHelper.RunSafely(EnchantCards(rewardsStack, room, Owner.Player));
        return base.AfterCombatEnd(room);
    }

    private async Task EnchantCards(IList rewardsStack, CombatRoom room, Player player)
    {
        var attempts = 0;
        while (rewardsStack.Count == 0 && attempts++ < 30) await Cmd.Wait(1);
        if (rewardsStack.Count == 0) return;

        var nimble = ModelDb.Enchantment<Nimble>();
        var sharp = ModelDb.Enchantment<Sharp>();
        foreach (var setStateObj in rewardsStack)
        {
            var set = Traverse.Create(setStateObj).Field("set").GetValue<RewardsSet>();
            if (set.Room != room) continue;
            foreach (var reward in set.Rewards)
            {
                if (reward is not CardReward cardReward) continue;
                var possibleCards = cardReward.Cards
                    .Where(card => nimble.CanEnchant(card) || sharp.CanEnchant(card))
                    .ToList();

                var cardToEnchant = player.PlayerRng.Rewards.NextItem(possibleCards);
                if (cardToEnchant is null) continue;

                var canNimble = nimble.CanEnchant(cardToEnchant);
                var canSharp = sharp.CanEnchant(cardToEnchant);
                var useNimble = canNimble && canSharp ? player.PlayerRng.Rewards.NextBool() : canNimble;

                if (useNimble) CardCmd.Enchant<Nimble>(cardToEnchant, Amount);
                else if (canSharp) CardCmd.Enchant<Sharp>(cardToEnchant, Amount);
            }
        }
    }
}