using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class Misprint() : BalatroCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(21, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public override Task BeforeAttack(AttackCommand command)
    {
        var attack = command.Results.First().FirstOrDefault();
        if (attack != null)
        {
            var newDmg = this.Owner.RunState.Rng.Niche.NextInt(1, attack.TotalDamage);
            var prop = attack.GetType().GetProperty(nameof(attack.TotalDamage));
            prop?.SetValue(attack, newDmg);
        }
        return base.BeforeAttack(command);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(11);
    }
}
