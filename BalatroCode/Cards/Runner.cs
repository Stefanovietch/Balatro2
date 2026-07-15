using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class Runner() : BalatroCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    private int _currentBlock = 4;

    private int _increasedBlock;
    
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
        new IntVar("BlockIncrease", 2M),
    ];

    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.Block, play);
        if (!HasCost012Cards()) 
            return;
        this.BuffFromPlay();
        if (this.DeckVersion is not Runner deckVersion)
            return;
        deckVersion.BuffFromPlay();
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars["BlockIncrease"].UpgradeValueBy(2);
    }

    protected override bool ShouldGlowGoldInternal => HasCost012Cards();

    private bool HasCost012Cards()
    {
        var amountList = PileType.Hand.GetPile(this.Owner).Cards.Where(c => !c.Equals(this))
            .Select(c => c.EnergyCost.GetAmountToSpend()).ToList();
        return amountList.Contains(0) && amountList.Contains(1) && amountList.Contains(2);
    }
    
    protected override void AfterDowngraded() => this.UpdateBlock();
    
    private void BuffFromPlay()
    {
        this.IncreasedBlock += this.DynamicVars["BlockIncrease"].IntValue;
        this.UpdateBlock();
    }
    
    private void UpdateBlock() => this.CurrentBlock = 1 + this.IncreasedBlock;
}
