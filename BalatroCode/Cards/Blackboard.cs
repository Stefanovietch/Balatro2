using Balatro.BalatroCode.Cards;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Cards;

public class Blackboard() : BalatroCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<VulnerablePower>(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        var amount = AllUncommonOrCommon() ? 3 : 1;
        await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target, this.DynamicVars.Power<VulnerablePower>().BaseValue * amount, this.Owner.Creature,  this);

    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Vulnerable.UpgradeValueBy(1);
    }

    protected override bool ShouldGlowGoldInternal => AllUncommonOrCommon();

    private bool AllUncommonOrCommon()
    {
        return PileType.Hand.GetPile(this.Owner).Cards.Count(c =>
            c.Rarity is not CardRarity.Common and not CardRarity.Uncommon) == 0;
    }
}
