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
        get => _currentBlock;
        set
        {
            AssertMutable();
            _currentBlock = value;
            DynamicVars.Block.BaseValue = _currentBlock;
        }
    }

    [SavedProperty]
    public int IncreasedBlock
    {
        get => _increasedBlock;
        set
        {
            AssertMutable();
            _increasedBlock = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(CurrentBlock, ValueProp.Move),
        new IntVar("BlockIncrease", 2M)
    ];


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        if (!HasCost012Cards())
            return;
        BuffFromPlay();
        if (DeckVersion is not Runner deckVersion)
            return;
        deckVersion.BuffFromPlay();
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BlockIncrease"].UpgradeValueBy(2);
    }

    protected override bool ShouldGlowGoldInternal => HasCost012Cards();

    private bool HasCost012Cards()
    {
        var amountList = PileType.Hand.GetPile(Owner).Cards.Where(c => !c.Equals(this))
            .Select(c => c.EnergyCost.GetAmountToSpend()).ToList();
        return amountList.Contains(0) && amountList.Contains(1) && amountList.Contains(2);
    }

    protected override void AfterDowngraded()
    {
        UpdateBlock();
    }

    private void BuffFromPlay()
    {
        IncreasedBlock += DynamicVars["BlockIncrease"].IntValue;
        UpdateBlock();
    }

    private void UpdateBlock()
    {
        CurrentBlock = 4 + IncreasedBlock;
    }
}