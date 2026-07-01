using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class Juggler() : BalatroCard(1,
    CardType.Power, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<JugglerPower>(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<JugglerPower>(choiceContext, this.Owner.Creature, this.DynamicVars["JugglerPower"].BaseValue, this.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Innate);
    }
}
