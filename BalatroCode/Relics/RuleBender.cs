using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Relics;

public class RuleBender() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    /*
    public override Task AfterCreatureAddedToCombat(Creature creature)
    {
        var hpDiff = creature.MaxHp - creature.CurrentHp;
        creature.SetMaxHpInternal(creature.MaxHp * 2);
        creature.SetCurrentHpInternal(creature.MaxHp - hpDiff);
        return base.AfterCreatureAddedToCombat(creature);
    }
    */
    
    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature) || Owner.PlayerCombatState?.TurnNumber > 1)
            return;
        Flash();
        foreach (var creature in combatState.Enemies)
        {
            var hpDiff = creature.MaxHp - creature.CurrentHp;
            creature.SetMaxHpInternal(creature.MaxHp * 2);
            creature.SetCurrentHpInternal(creature.MaxHp - hpDiff);
        }
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer,
        DamageResult result, ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (dealer == null || dealer != Owner.Creature) return;
        if (cardSource is not { Type: CardType.Attack }) return;
        await CreatureCmd.GainBlock(Owner.Creature, result.TotalDamage / 2M, ValueProp.Move, null);
    }
}