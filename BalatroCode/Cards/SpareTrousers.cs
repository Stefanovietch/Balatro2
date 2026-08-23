using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class SpareTrousers() : BalatroCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.Self)
{
    private int _increasedDamage;
    private int _currentDamage = 6;

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

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(CurrentDamage, ValueProp.Move),
        new IntVar("DamageIncrease", 2)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Fatal)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        
        
        var attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
            .WithHitCount(2)
            .TargetingRandomOpponents(CombatState, false)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (!attackCommand.Results.SelectMany(r => r).Any(r =>
                r.WasTargetKilled && r.Receiver.Powers.All(p => p.ShouldOwnerDeathTriggerFatal())))
            return;
        var intValue = DynamicVars["DamageIncrease"].IntValue;
        BuffFromFatal(intValue);
        if (DeckVersion is not SpareTrousers deckVersion)
            return;
        deckVersion.BuffFromFatal(intValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DamageIncrease"].UpgradeValueBy(2);
    }

    protected override void AfterDowngraded()
    {
        UpdateDamage();
    }

    private void BuffFromFatal(int extraDamage)
    {
        IncreasedDamage += extraDamage;
        UpdateDamage();
    }

    private void UpdateDamage()
    {
        CurrentDamage = 13 + IncreasedDamage;
    }
}