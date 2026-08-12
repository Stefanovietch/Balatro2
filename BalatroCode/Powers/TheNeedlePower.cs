using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Powers;

public class TheNeedlePower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;
    
    public BlindType BlindType => BlindType.TheNeedle;
    
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, 10, applier, cardSource, true);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        var enumerable = participants.ToList();
        if (side != CombatSide.Player) return;
        await PowerCmd.TickDownDuration(this);
        if (this.Amount <= 1)
        {
            foreach (var p in enumerable.Where(c => c is { IsPlayer: true, IsAlive: true, Player.Character: Character.Balatro }))
            {
                if (p.Player == null) continue;
                await CreatureCmd.Kill(p.Player.Creature, true);
            }
        }
    }

    public override int DisplayAmount => this.Amount - 1;
}