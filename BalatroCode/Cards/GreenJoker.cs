using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class GreenJoker() : BalatroCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    private decimal _bonus;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8, ValueProp.Move),
        new("Bonus", 3)
    ];

    private decimal ExtraDamageFromPlays
    {
        get => _bonus;
        set
        {
            AssertMutable();
            _bonus = value;
        }
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card.Pile?.Type == PileType.Discard && card.Equals(this))
        {
            if (oldPileType == PileType.Play) Buff(DynamicVars["Bonus"].BaseValue);
            else Buff(-DynamicVars["Bonus"].BaseValue);
        }

        return base.AfterCardChangedPiles(card, oldPileType, clonedBy);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Bonus"].UpgradeValueBy(3);
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        var damage = DynamicVars.Damage;
        damage.BaseValue = damage.BaseValue + ExtraDamageFromPlays;
    }

    private void Buff(decimal bonus)
    {
        var damage = DynamicVars.Damage;
        damage.BaseValue = damage.BaseValue + bonus;
        ExtraDamageFromPlays += bonus;
    }
}