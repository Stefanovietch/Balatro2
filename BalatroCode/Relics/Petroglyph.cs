using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Balatro.BalatroCode.Relics;

public class Petroglyph() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;


    public override async Task AfterCreatureAddedToCombat(Creature creature)
    {
        this.Flash();
        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), creature, (decimal) (creature.MaxHp*0.1), false);
    }
}