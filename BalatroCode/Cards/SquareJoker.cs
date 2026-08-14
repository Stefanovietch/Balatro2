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
        new IntVar("BlockIncrease", 2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        if (!Is4thPlay()) return;
        var intValue = DynamicVars["BlockIncrease"].IntValue;
        BuffFrom4thPlay(intValue);
        if (DeckVersion is not SquareJoker deckVersion) return;
        deckVersion.BuffFrom4thPlay(intValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BlockIncrease"].UpgradeValueBy(2);
    }

    private bool Is4thPlay()
    {
        return CombatManager.Instance.History.CardPlaysFinished.Count(c =>
            c.HappenedThisTurn(CombatState) && c.CardPlay.Card.Owner == Owner) == 3;
    }

    protected override bool ShouldGlowGoldInternal => Is4thPlay();

    protected override void AfterDowngraded()
    {
        UpdateBlock();
    }

    private void BuffFrom4thPlay(int extraBlock)
    {
        IncreasedBlock += extraBlock;
        UpdateBlock();
    }

    private void UpdateBlock()
    {
        CurrentBlock = 4 + IncreasedBlock;
    }
}