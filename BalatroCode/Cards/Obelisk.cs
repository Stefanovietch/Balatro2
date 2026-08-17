using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class Obelisk() : BalatroCard(2,
    CardType.Skill, CardRarity.Rare,
    TargetType.AllEnemies)
{
    private int _currentDamage = 4;
    private int _currentBlock = 8;

    private int _increasedDamage;
    private int _increasedBlock;

    [SavedProperty]
    public int CurrentDamage
    {
        get => _currentDamage;
        set
        {
            AssertMutable();
            _currentDamage = value;
            DynamicVars.Damage.BaseValue = _currentDamage;
        }
    }

    [SavedProperty]
    public int IncreasedDamage
    {
        get => _increasedDamage;
        set
        {
            AssertMutable();
            _increasedDamage = value;
        }
    }

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
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];


    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(CurrentDamage, ValueProp.Move),
        new IntVar("DamageIncrease", 1M),
        new BlockVar(CurrentBlock, ValueProp.Move),
        new IntVar("BlockIncrease", 2M)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        IReadOnlyList<CardModel> options =
        [
            CombatState.CreateCard(ModelDb.Card<Blockade>(), this.Owner),
            CombatState.CreateCard(ModelDb.Card<Assault>(), this.Owner),
            CombatState.CreateCard(ModelDb.Card<Enhance>(), this.Owner),
        ];
        var option1 = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, Owner, false);
        //await Task.Yield();
        var option2 = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, Owner, false);

        foreach (var option in (List<CardModel?>)[option1, option2])
            switch (option)
            {
                case Blockade:
                    await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
                    break;
                case Assault:
                    await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
                        .TargetingAllOpponents(CombatState)
                        .WithHitFx("vfx/vfx_attack_slash")
                        .Execute(choiceContext);
                    break;
                case Enhance:
                    Buff();
                    if (DeckVersion is not Obelisk deckVersion)
                        return;
                    deckVersion.Buff();
                    break;
            }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DamageIncrease"].UpgradeValueBy(1);
        DynamicVars["BlockIncrease"].UpgradeValueBy(1);
    }

    private void Buff()
    {
        IncreasedBlock += DynamicVars["BlockIncrease"].IntValue;
        IncreasedDamage += DynamicVars["DamageIncrease"].IntValue;
        UpdateDamageBlock();
    }

    protected override void AfterDowngraded()
    {
        UpdateDamageBlock();
    }

    private void UpdateDamageBlock()
    {
        CurrentDamage = 4 + IncreasedDamage;
        CurrentBlock = 8 + IncreasedBlock;
    }
}