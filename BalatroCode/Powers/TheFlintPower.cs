using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Powers;

public class TheFlintPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public BlindType BlindType => BlindType.TheFlint;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var players = Owner.CombatState?.RunState.Players;
        if (players is null) return;
        foreach (var player in players)
        {
            if (player.Character is not Character.Balatro) continue;
            await PowerCmd.Apply<FlintVulnerablePower>(new ThrowingPlayerChoiceContext(), player.Creature, 1, null,
                null);
            await PowerCmd.Apply<FlintWeakPower>(new ThrowingPlayerChoiceContext(), player.Creature, 1, null, null);
        }
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        var players = Owner.CombatState?.RunState.Players;
        if (players is null) return;
        foreach (var player in players)
        {
            if (player.Character is not Character.Balatro) continue;
            await PowerCmd.Remove<FlintVulnerablePower>(player.Creature);
            await PowerCmd.Remove<FlintWeakPower>(player.Creature);
        }
    }
}