using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Cards;

public class Vampire() : BalatroCard(-1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int count = this.ResolveEnergyXValue();
        if (this.IsUpgraded)
            ++count;
        ArgumentNullException.ThrowIfNull(play.Target);
        await PowerCmd.Apply<StrengthPower>(choiceContext, play.Target, -count, this.Owner.Creature, this);
        await PowerCmd.Apply<StrengthPower>(choiceContext, this.Owner.Creature, count, this.Owner.Creature, this);
    }
}
