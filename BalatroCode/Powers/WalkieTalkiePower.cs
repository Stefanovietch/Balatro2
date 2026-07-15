using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Powers;

public class WalkieTalkiePower() : BalatroPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != this.Owner.Player || cardPlay.Card.EnergyCost.GetAmountToSpend() != 1) return;
        await CreatureCmd.GainBlock(this.Owner, Amount, ValueProp.Move, cardPlay);
        Creature? target = this.Owner.Player.RunState.Rng.CombatTargets.NextItem(this.CombatState.HittableEnemies);
        if (target == null) return;
        await CreatureCmd.Damage(choiceContext, target, Amount, ValueProp.Unpowered, this.Owner, null);
    }
}