using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class Bootstraps() : BalatroCard(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        ..MakeCalculatedDamage(0, (card, target) => card.Owner.Gold),
        new("GoldLose", 100)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.CalculatedDamage.Calculate(play.Target)).FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_coin_explosion_jumbo")
            .Execute(choiceContext);
        await PlayerCmd.LoseGold(DynamicVars["GoldLose"].BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["GoldLose"].UpgradeValueBy(-20);
    }
}