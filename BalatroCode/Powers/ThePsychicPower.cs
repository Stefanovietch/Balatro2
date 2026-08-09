using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Powers;

public class ThePsychicPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;
    
    public BlindType BlindType => BlindType.ThePsychic;
    
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var countBalatro = this.Owner.CombatState?.Players.Count(p => p.Character is Character.Balatro) ?? 1;
        await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, 5 * countBalatro, applier, cardSource, true);
    }

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (this.Amount <= 1) return false;
        return base.ShouldPlay(card, autoPlayType);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Character is Character.Balatro) await PowerCmd.Decrement(this);
    }

    public override int DisplayAmount => this.Amount - 1;
}