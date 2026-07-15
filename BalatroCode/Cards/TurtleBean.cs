using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class TurtleBean() : BalatroCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    private int _currentDraw = 5;
    
    private int CurrentDraw
    {
        get => this._currentDraw;
        set
        {
            this.AssertMutable();
            if ((this.IsUpgraded ? 1 : 0) <= value) this._currentDraw = value;
            this.DynamicVars.Cards.BaseValue = value;
        }
    }
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(5)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CardPileCmd.Draw(choiceContext, this.DynamicVars.Cards.BaseValue, this.Owner);
        this.CurrentDraw -= 1;
    }

    protected override void OnUpgrade()
    {
        if (CurrentDraw <= 0) CurrentDraw = 1;
    }
}
