using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Powers;

public class TheWaterPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public BlindType BlindType => BlindType.TheWater;

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (autoPlayType == AutoPlayType.SlyDiscard && card.Owner.Character is Character.Balatro) return false;
        return base.ShouldPlay(card, autoPlayType);
    }

    public override bool ShouldDiscard(CardModel card)
    {
        if (card.Owner.Character is Character.Balatro) return false;
        return base.ShouldDiscard(card);
    }
}