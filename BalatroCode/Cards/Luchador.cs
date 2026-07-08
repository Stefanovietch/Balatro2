using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Cards;

public class Luchador() : BalatroCard(2,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<StrengthPower>(8)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await PowerCmd.Apply<StrengthPower>(choiceContext, play.Target, -this.DynamicVars.Strength.BaseValue, this.Owner.Creature, this);
        await CardPileCmd.RemoveFromCombat(this);
        if (this.DeckVersion is not Luchador deckVersion)
            return;
        await CardPileCmd.RemoveFromDeck(deckVersion);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Strength.UpgradeValueBy(3);
    }
}
