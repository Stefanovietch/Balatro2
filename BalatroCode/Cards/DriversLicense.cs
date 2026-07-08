using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Cards;

public class DriversLicense() : BalatroCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<StrengthPower>(3),
        new PowerVar<DexterityPower>(3)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (HasRareCards())
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, this.Owner.Creature,
                this.DynamicVars.Strength.BaseValue, this.Owner.Creature, this);
            await PowerCmd.Apply<DexterityPower>(choiceContext, this.Owner.Creature,
                this.DynamicVars.Dexterity.BaseValue, this.Owner.Creature, this);
        }
    }

    protected override bool ShouldGlowGoldInternal => HasRareCards();

    protected override void OnUpgrade()
    {
        this.DynamicVars.Strength.UpgradeValueBy(1M);
        this.DynamicVars.Dexterity.UpgradeValueBy(1M);
    }

    private bool HasRareCards()
    {
        return PileType.Deck.GetPile(this.Owner).Cards.Count(c => c.Rarity == CardRarity.Rare) >= 4;
    }
}
