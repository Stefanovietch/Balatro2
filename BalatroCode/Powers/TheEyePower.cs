using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
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

    private Dictionary<Player, CardType> LastCardTypePlayed { get; } = new();

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner.Character is not Character.Balatro) return base.ShouldPlay(card, autoPlayType);
        if (!LastCardTypePlayed.ContainsKey(card.Owner)) return true;
        if (LastCardTypePlayed.TryGetValue(card.Owner, out var type)) return type != card.Type;
        return base.ShouldPlay(card, autoPlayType);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Character is Character.Balatro) LastCardTypePlayed[cardPlay.Card.Owner] = cardPlay.Card.Type;
        return base.AfterCardPlayed(choiceContext, cardPlay);
    }
}