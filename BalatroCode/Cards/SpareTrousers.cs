using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class SpareTrousers() : BalatroCard(1,
    CardType.Attack, CardRarity.Basic,
    TargetType.Self)
{
    
    private int _increasedDamage;
    private int _currentDamage = 6;
    
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
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(this.CurrentDamage, ValueProp.Move),
        new IntVar("DamageIncrease", 2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(this.CombatState);
        AttackCommand attackCommand = await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard(this)
            .WithHitCount(2)
            .TargetingRandomOpponents(this.CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        
        if (!attackCommand.Results.SelectMany(r => r).Any(r => r.WasTargetKilled && r.Receiver.Powers.All(p => p.ShouldOwnerDeathTriggerFatal())))
            return;
        int intValue = this.DynamicVars["DamageIncrease"].IntValue;
        this.BuffFromFatal(intValue);
        if (this.DeckVersion is not SpareTrousers deckVersion)
            return;
        deckVersion.BuffFromFatal(intValue);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars["DamageIncrease"].UpgradeValueBy(2);
    }
    
    protected override void AfterDowngraded() => this.UpdateDamage();

    private void BuffFromFatal(int extraDamage)
    {
        this.IncreasedDamage += extraDamage;
        this.UpdateDamage();
    }

    private void UpdateDamage() => this.CurrentDamage = 13 + this.IncreasedDamage;
}
