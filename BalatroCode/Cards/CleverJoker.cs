using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class CleverJoker() : BalatroCard(1,
    CardType.Skill, CardRarity.Basic,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(10,ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (!LastCardIsSkill()) return;
        Decimal num = await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.Block, play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(4);
    }

    protected override bool ShouldGlowGoldInternal => LastCardIsSkill();

    private bool LastCardIsSkill()
    {
        int playPileSize = PileType.Play.GetPile(this.Owner).Cards.Count;
        if (playPileSize <= 1) return false;
        return PileType.Play.GetPile(this.Owner).Cards.ElementAt(playPileSize - 2).Type == CardType.Skill;
    }
}
