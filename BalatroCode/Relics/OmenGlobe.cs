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
        if (potion.Owner != this.Owner || !CombatManager.Instance.IsInProgress)
            return;
        this.Flash();
        List<CardModel> list = CardFactory.GetDistinctForCombat(this.Owner, ModelDb.CardPool<ColorlessCardPool>().GetUnlockedCards(this.Owner.UnlockState, this.Owner.RunState.CardMultiplayerConstraint), 1, this.Owner.RunState.Rng.CombatCardGeneration).ToList<CardModel>();
        IReadOnlyList<CardPileAddResult> combat = await CardPileCmd.AddGeneratedCardsToCombat((IEnumerable<CardModel>) list, PileType.Hand, this.Owner);
    }
}