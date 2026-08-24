using System.Reflection;
using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Powers;

public class PareidoliaPower() : BalatroPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (this.Owner.Player is not {} player) return Task.CompletedTask;
        foreach (var card in PileType.Hand.GetPile(player).Cards)
        {
            card.PortraitBorder.EmitChanged();
        }
        return base.AfterApplied(applier, cardSource);
    }
}