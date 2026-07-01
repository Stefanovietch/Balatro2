using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Powers;

public class LoyaltyCardPower() : BalatroPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Decrement(this);
        if (this.Amount >= 1) return;
        await PowerCmd.ModifyAmount(choiceContext,this, 6, null, null);
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if(this.Amount <= 1) cardPlay.Card.ModifyDamageMultiplicative(cardPlay.Target, 4, cardPlay.Card.DynamicVars.Damage.Props, this.Owner, cardPlay.Card);
        return base.BeforeCardPlayed(cardPlay);
    }
}