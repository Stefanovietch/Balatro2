using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class SquareJoker() : BalatroCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    private int _increasedBlock;
    private int _currentBlock = 4;
    
    [SavedProperty]
    public int CurrentBlock
    {
        get => this._currentBlock;
        set
        {
            this.AssertMutable();
            this._currentBlock = value;
            this.DynamicVars.Block.BaseValue = this._currentBlock;
        }
    }

    [SavedProperty]
    public int IncreasedBlock
    {
        get => this._increasedBlock;
        set
        {
            this.AssertMutable();
            this._increasedBlock = value;
        }
    }
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(this.CurrentBlock, ValueProp.Move),
        new IntVar("BlockIncrease", 2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.Block, play);
        if(!Is4thPlay()) return;
        int intValue = this.DynamicVars["BlockIncrease"].IntValue;
        this.BuffFrom4thPlay(intValue);
        if (this.DeckVersion is not SquareJoker deckVersion) return;
        deckVersion.BuffFrom4thPlay(intValue);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars["BlockIncrease"].UpgradeValueBy(2);

    }

    private bool Is4thPlay()
    {
        return CombatManager.Instance.History.CardPlaysFinished.Count(c =>
            c.HappenedThisTurn(this.CombatState) && c.CardPlay.Card.Owner == this.Owner) == 3;
    }

    protected override bool ShouldGlowGoldInternal => Is4thPlay();

    protected override void AfterDowngraded() => this.UpdateBlock();

    private void BuffFrom4thPlay(int extraBlock)
    {
        this.IncreasedBlock += extraBlock;
        this.UpdateBlock();
    }

    private void UpdateBlock() => this.CurrentBlock = 4 + this.IncreasedBlock;
}
