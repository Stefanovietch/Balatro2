using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Cards;

public class Throwback() : BalatroCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        ..MakeCalculatedVar("QuestionMarks", 0, (c, _) => c.Owner.Character is Character.Balatro balatro ? balatro.QuestionMarksVisited.Get(c.Owner) : 0)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target, this.DynamicVars["QuestionMarks"].BaseValue, this.Owner.Creature,  this);
        await PowerCmd.Apply<WeakPower>(choiceContext, play.Target, this.DynamicVars["QuestionMarks"].BaseValue, this.Owner.Creature,  this);

    }

    protected override void OnUpgrade()
    {
        this.DynamicVars["QuestionMarks"].UpgradeValueBy(1);
    }
}
