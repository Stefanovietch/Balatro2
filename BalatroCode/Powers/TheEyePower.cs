using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Powers;

public class TheEyePower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;
    public BlindType BlindType => BlindType.TheEye;
    
    private CardType _lastCardTypePlayed =  CardType.None;
    
    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        return this._lastCardTypePlayed != card.Type;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        this._lastCardTypePlayed = cardPlay.Card.Type;
        return base.AfterCardPlayed(choiceContext, cardPlay);
    }
}