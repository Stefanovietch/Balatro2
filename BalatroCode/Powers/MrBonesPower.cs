using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;

namespace Balatro.BalatroCode.Powers;

public class MrBonesPower() : BalatroPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(50)
    ];

    public override bool ShouldDieLate(Creature creature)
    {
        return creature != Owner;
    }

    public override async Task AfterPreventingDeath(Creature creature)
    {
        Flash();
        await PowerCmd.Decrement(this);
        await CreatureCmd.Heal(creature, Math.Max(1M, creature.MaxHp * (DynamicVars.Heal.BaseValue / 100M)));

        if (Owner.Player == null) return;
        var mrBonesCards = PileType.Deck.GetPile(Owner.Player).Cards.Where(c => c is MrBones).ToList();
        var unUpgradedMrBones = mrBonesCards.Where(c => !c.IsUpgraded).ToList();
        var mrBonesCard = unUpgradedMrBones.Count != 0 ? unUpgradedMrBones.First() : mrBonesCards.FirstOrDefault();
        if (mrBonesCard != null) await CardPileCmd.RemoveFromDeck(mrBonesCard);
    }
}