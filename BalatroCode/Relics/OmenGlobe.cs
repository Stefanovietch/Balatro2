using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Relics;

public class OmenGlobe() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override async Task AfterPotionUsed(PotionModel potion, Creature? target)
    {
        if (potion.Owner != Owner || !CombatManager.Instance.IsInProgress)
            return;
        Flash();
        var list = CardFactory.GetDistinctForCombat(Owner,
            ModelDb.CardPool<ColorlessCardPool>()
                .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint), 1,
            Owner.RunState.Rng.CombatCardGeneration).ToList<CardModel>();
        var combat = await CardPileCmd.AddGeneratedCardsToCombat((IEnumerable<CardModel>)list, PileType.Hand, Owner);
    }
}