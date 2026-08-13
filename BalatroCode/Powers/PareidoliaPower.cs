using System.Reflection;
using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Powers;

public class PareidoliaPower() : BalatroPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        var field = typeof(CardModel).GetField(
            "<Type>k__BackingField",
            BindingFlags.Instance | BindingFlags.NonPublic);

        field?.SetValue(cardPlay.Card, CardType.Power);

        return base.BeforeCardPlayed(cardPlay);
    }
}