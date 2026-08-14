using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Relics;

public class RuleBender() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override async Task AfterCreatureAddedToCombat(Creature creature)
    {
        await CreatureCmd.SetMaxAndCurrentHp(creature, creature.MaxHp * 2);
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer,
        DamageResult result, ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (dealer == null || dealer != Owner.Creature) return;
        if (cardSource is not { Type: CardType.Attack }) return;
        await CreatureCmd.GainBlock(Owner.Creature, (decimal)result.TotalDamage, ValueProp.Move, null);
    }
}