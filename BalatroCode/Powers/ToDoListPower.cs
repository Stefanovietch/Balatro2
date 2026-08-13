using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Powers;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Powers;


public class ToDoListPower() : BalatroPower, IRandomType
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DisplayVar<AncientJoker>("Type", card =>  ((IRandomType) card).GetTypeString())
    ];
    public CardType CurrentType { get; set; } = CardType.None;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (this.Owner.Player == null) return;
        if (cardPlay.Card.Type == CurrentType) await PlayerCmd.GainGold(Amount, this.Owner.Player);
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        ((IRandomType) this).SetRandomType();
        return base.AfterApplied(applier, cardSource);
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        ((IRandomType) this).SetRandomType();
        return base.AfterPlayerTurnStart(choiceContext, player);
    }
}