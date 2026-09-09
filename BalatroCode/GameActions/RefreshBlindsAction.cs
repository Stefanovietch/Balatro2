using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Rooms;

namespace Balatro.BalatroCode.GameActions;

public class RefreshBlindsAction : GameAction
{
    private readonly Player _player;
    private readonly int _rerollCost;
    private readonly ICombatState _combatState;
    
    public override ulong OwnerId => _player.NetId;

    public override GameActionType ActionType => GameActionType.CombatPlayPhaseOnly;

    public RefreshBlindsAction(Player player, int rerollCost)
    {
        _player = player;
        _rerollCost = rerollCost;
        _combatState = player.Creature.CombatState!;
    }

    protected override async Task ExecuteAction()
    {
        if (_player.Gold < _rerollCost) return;

        var enemy = _combatState.Enemies.FirstOrDefault(creature =>
            creature is { IsPet: false, CanReceivePowers: true, IsPlayer: false, IsPrimaryEnemy: true });
        if (enemy == null) return;

        var blindPowers = enemy.Powers.Where(p => p is IBlindPower).ToList();
        if (blindPowers.Count == 0) return;

        var blindTypes = blindPowers.Select(p => ((IBlindPower)p).BlindType).ToList();

        await PlayerCmd.LoseGold(_rerollCost, _player, GoldLossType.Spent);

        // RNG draw now happens inside ExecuteAction, so every peer draws identically.
        var blindPower = _combatState.RunState.Rng.MonsterAi.NextItem(blindPowers);
        await PowerCmd.Remove(blindPower);

        var isBoss = _combatState.RunState.CurrentRoom?.RoomType == RoomType.Boss;
        var newBlind = BlindMethods.GetRandomBlindPower(enemy, isBoss, blindTypes).ToMutable();

        await PowerCmd.Apply(new ThrowingPlayerChoiceContext(), newBlind, enemy, 1, null, null);
    }

    public override INetAction ToNetAction()
    {
        return new NetRefreshBlindsAction { RerollCost = _rerollCost };
    }
}

public struct NetRefreshBlindsAction : INetAction, IPacketSerializable
{
    public int RerollCost;

    public GameAction ToGameAction(Player player)
    {
        return new RefreshBlindsAction(player, RerollCost);
    }

    public void Serialize(PacketWriter writer)
    {
        writer.WriteInt(RerollCost);
    }

    public void Deserialize(PacketReader reader)
    {
        RerollCost = reader.ReadInt();
    }

    public override string ToString()
    {
        return $"NetRefreshBlindsAction ({RerollCost})";
    }
}