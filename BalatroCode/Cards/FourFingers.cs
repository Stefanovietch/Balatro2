using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class FourFingers() : BalatroCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.RandomEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar("Damage1", 1, ValueProp.Move),
        new DamageVar("Damage2", 2, ValueProp.Move),
        new DamageVar("Damage3", 3, ValueProp.Move),
        new DamageVar("Damage4", 4, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(CombatState);

        List<DynamicVar> damageList = [DynamicVars["Damage1"], DynamicVars["Damage2"], DynamicVars["Damage3"], DynamicVars["Damage4"]];
        
        foreach (var damageVar in damageList)
            if (IsUpgraded)
            {
                ArgumentNullException.ThrowIfNull(play.Target);
                await DamageCmd.Attack(damageVar.BaseValue).FromCard(this)
                    .Targeting(play.Target)
                    .WithHitFx("vfx/vfx_scratch")
                    .Execute(choiceContext);
            }
            else
                await DamageCmd.Attack(damageVar.BaseValue).FromCard(this)
                    .TargetingRandomOpponents(CombatState)
                    .WithHitFx("vfx/vfx_scratch")
                    .Execute(choiceContext);
    }

    public override TargetType TargetType => IsUpgraded ? TargetType.AnyEnemy : TargetType.RandomEnemy;

    protected override void OnUpgrade()
    {
    }
}