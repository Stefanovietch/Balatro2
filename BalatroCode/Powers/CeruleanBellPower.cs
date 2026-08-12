using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Balatro.BalatroCode.Powers;


public class CeruleanBellPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public BlindType BlindType => BlindType.CeruleanBell;
    
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        var enumerable = participants.ToList();
        if (side != CombatSide.Enemy) return;
        foreach (var p in enumerable.Where(c => c is { IsPlayer: true, IsAlive: true, Player.Character: Character.Balatro }))
        {
            if (p.Player == null) continue;
            var card = PileType.Hand.GetPile(p.Player).Cards.TakeRandom(1, p.Player.RunState.Rng.CombatCardSelection).FirstOrDefault();
            if (card == null) continue;
            await CardCmd.AutoPlay(new ThrowingPlayerChoiceContext(), card, null);
            await PlayerCmd.LoseEnergy(card.EnergyCost.GetAmountToSpend(), p.Player);
        }
    }
}