using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Relics;

public class Recyclomancy() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;

    private int _cardsDiscarded;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount => !IsCanonical ? _cardsDiscarded : 0;

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner == Owner) ++_cardsDiscarded;
        if (_cardsDiscarded < 7) return;
        _cardsDiscarded = 0;
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, 2, Owner.Creature, null);
    }
}