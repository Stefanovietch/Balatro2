using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Relics;

public class Recyclomancy() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;

    private int _cardsDiscarded;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;
    
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public override int DisplayAmount => IsCanonical ? 0 : _cardsDiscarded;

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner != Owner) return;
        _cardsDiscarded++;
        this.InvokeDisplayAmountChanged();
        if (_cardsDiscarded < 4) return;
        _cardsDiscarded = 0;
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, 2, Owner.Creature, null);
        this.InvokeDisplayAmountChanged();
    }
}