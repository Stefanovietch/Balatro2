using Balatro.BalatroCode.GameActions;
using Balatro.BalatroCode.Powers;
using Balatro.BalatroCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.UI;

public partial class NRerollRetconButton : NRerollButton
{
    private NCombatUi? _screen;
    
    public void Initialize(NCombatUi screen)
    {
        Name = "RerollRetconButton";
        MouseFilter = MouseFilterEnum.Stop;
        FocusMode = FocusModeEnum.All;
        _screen = screen;
        Hide();
    }

    public override void _Ready()
    {
        Character.Balatro.CardPlayed -= Hide;
        Character.Balatro.CardPlayed += Hide;
        Character.Balatro.TurnEnd -= Hide;
        Character.Balatro.TurnEnd += Hide;
        Character.Balatro.CombatStart -= CombatStartCheck;
        Character.Balatro.CombatStart += CombatStartCheck;
        base._Ready();
    }

    private void CombatStartCheck()
    {
        var combatState = Traverse.Create(_screen).Field("_state").GetValue<CombatState>();
        if (combatState == null) return;

        var player = LocalContext.GetMe(combatState);
        if (combatState.RunState.CurrentRoom?.RoomType is RoomType.Boss or RoomType.Elite && player?.GetRelic<Retcon>() != null) Show();
    }

    protected override async void OnRelease()
    {
        if (_screen == null) return;
        
        var combatState = Traverse.Create(_screen).Field("_state").GetValue<CombatState>();
        if (combatState == null) return;

        var player = LocalContext.GetMe(combatState);
        
        if (player == null) return;
        if (player.Gold < RerollCost) return;
        
        RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new RefreshBlindsAction(player, RerollCost));
    }

    private async Task RefreshBlinds(NCombatUi screen, CombatState combatState)
    {
        var enemy = combatState.Enemies.FirstOrDefault(creature =>
            creature is { IsPet: false, CanReceivePowers: true, IsPlayer: false, IsPrimaryEnemy: true });
        if (enemy == null) return;
        var blindPowers = enemy.Powers.Where(p => p is IBlindPower).ToList();
        if (blindPowers.Count == 0) return;
        var blindTypes = blindPowers.Select(p => ((IBlindPower)p).BlindType).ToList();
        var blindPower = enemy.CombatState?.RunState.Rng.MonsterAi.NextItem(blindPowers);
        await PowerCmd.Remove(blindPower);
        await PowerCmd.Apply(new ThrowingPlayerChoiceContext(), BlindMethods.GetRandomBlindPower(enemy, 
            combatState.RunState.CurrentRoom?.RoomType == RoomType.Boss, blindTypes).ToMutable(), enemy, 1, null,null);
    }
    
    public override void _ExitTree()
    {
        Character.Balatro.CardPlayed -= Hide;
        Character.Balatro.TurnEnd -= Hide;
        Character.Balatro.CombatStart -= CombatStartCheck;
        
        base._ExitTree();
    }
    
}