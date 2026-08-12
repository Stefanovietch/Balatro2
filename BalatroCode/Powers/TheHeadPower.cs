using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Balatro.BalatroCode.Powers;

public class TheHeadPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;
    
    public BlindType BlindType => BlindType.TheHead;
    
    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        var enumerable = participants.ToList();
        if (side != CombatSide.Enemy) return;
        await PowerCmd.Apply<SlowPower>(choiceContext, enumerable.Where(c => c is { IsPlayer: true, IsAlive: true, Player.Character: Character.Balatro }), 1, this.Owner,null);
    }
    
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var players = this.Owner.CombatState?.RunState.Players;
        if (players is null) return;
        await PowerCmd.Apply<SlowPower>(new ThrowingPlayerChoiceContext(), players.Where(p => p.Character is Character.Balatro).Select(p => p.Creature), 1, this.Owner, null);
    }
}