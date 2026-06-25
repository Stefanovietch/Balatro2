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

    public override int DisplayAmount => !this.IsCanonical ? this._cardsDiscarded : 0;

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner == this.Owner) ++this._cardsDiscarded;
        if (this._cardsDiscarded < 7) return;
        this._cardsDiscarded = 0;
        await PowerCmd.Apply<StrengthPower>(choiceContext, this.Owner.Creature,2, this.Owner.Creature,  null);

    }
}