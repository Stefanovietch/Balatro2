using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
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
        get => this._currentDamage;
        set
        {
            this.AssertMutable();
            this._currentDamage = value;
            this.DynamicVars.Damage.BaseValue = this._currentDamage;
        }
    }

    [SavedProperty]
    public int IncreasedDamage
    {
        get => this._increasedDamage;
        set
        {
            this.AssertMutable();
            this._increasedDamage = value;
        }
    }
    
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
        new DamageVar(this.CurrentDamage, ValueProp.Move),
        new IntVar("DamageIncrease", 1M),
        new BlockVar(this.CurrentBlock, ValueProp.Move),
        new IntVar("BlockIncrease", 2M),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(this.CombatState);
        IReadOnlyList<CardModel> options = [new Blockade(), new Assault(), new Enhance()];
        var option1 = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, this.Owner, true);
        var option2 = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, this.Owner, true);
        foreach (var option in (List<CardModel>) [option1, option2])
        {
            switch (option)
            {
                case Blockade:
                    await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.Block, play);
                    break;
                case Assault:
                    await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard(this)
                        .TargetingAllOpponents(this.CombatState)
                        .WithHitFx("vfx/vfx_attack_slash")
                        .Execute(choiceContext);
                    break;
                case Enhance:
                    Buff();
                    break;
            }
        }
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars["DamageIncrease"].UpgradeValueBy(1);
        this.DynamicVars["BlockIncrease"].UpgradeValueBy(1);
    }
    
    private void Buff()
    {
        this.IncreasedBlock += this.DynamicVars["BlockIncrease"].IntValue;;
        this.IncreasedBlock += this.DynamicVars["BlockIncrease"].IntValue;;
        this.UpdateDamageBlock();
    }
    protected override void AfterDowngraded() => this.UpdateDamageBlock();

    private void UpdateDamageBlock()
    {
        this.CurrentDamage = 4 + this.IncreasedDamage;
        this.CurrentBlock = 8 + this.IncreasedBlock;
    }
}
