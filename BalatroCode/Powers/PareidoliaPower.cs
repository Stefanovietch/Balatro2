using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Balatro.BalatroCode.Powers;

public class PareidoliaPower() : BalatroPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        var prop = cardPlay.Card.GetType().GetProperty(nameof(cardPlay.Card.Type));
        prop?.SetValue(cardPlay.Card, CardType.Power);
        return base.BeforeCardPlayed(cardPlay);
    }
}