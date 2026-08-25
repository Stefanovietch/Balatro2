using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Cards;

public class GiftCard() : BalatroCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ArtifactPower>(1),
        new PowerVar<GiftCardPower>(2)
    ];

    public override bool CanBeGeneratedInCombat => false;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ArtifactPower>(),
        ..HoverTipFactory.FromEnchantment<Sharp>(),
        ..HoverTipFactory.FromEnchantment<Nimble>()
    ];
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<ArtifactPower>(choiceContext, Owner.Creature, DynamicVars["ArtifactPower"].BaseValue,
            Owner.Creature, this);
        await PowerCmd.Apply<GiftCardPower>(choiceContext, Owner.Creature, DynamicVars["GiftCardPower"].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ArtifactPower"].UpgradeValueBy(1);
        DynamicVars["GiftCardPower"].UpgradeValueBy(1);

    }
}