using Balatro.BalatroCode.Cards;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class Fibonacci() : BalatroCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DisplayVar<Fibonacci>("Turn",f => f.Owner.PlayerCombatState?.TurnNumber.ToString() ?? ""),
        ..MakeCalculatedDamage(0,(card, _) => Fib(card.Owner.PlayerCombatState?.TurnNumber ?? 0))
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(this.DynamicVars.CalculatedDamage.Calculate(play.Target)).FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Exhaust);
    }
    
    private static int Fib(int n) {
        return (n < 2)? n : Fib(n - 1) + Fib(n - 2);
    }
}
