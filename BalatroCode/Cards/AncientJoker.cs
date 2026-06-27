using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Character;
using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

[Pool(typeof(BalatroCardPool))]
public class AncientJoker() : BalatroCard(0,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy), IRandomType
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4, ValueProp.Move),
        new DisplayVar<AncientJoker>("Type", card =>  ((IRandomType) card).GetTypeString())
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(2);
    }
    
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        if (cardSource != this) return base.ModifyDamageMultiplicative(target, amount, props, dealer, cardSource);
        return (decimal) Math.Pow(1.5, CombatManager.Instance.History.CardPlaysFinished.Count(c => c.HappenedThisTurn(cardSource.CombatState) && c.CardPlay.Card.Type == CardType.Attack && c.CardPlay.Card.Owner == cardSource.Owner));
    }

    public CardType CurrentType { get; set; } = CardType.None;
    
}
