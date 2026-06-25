using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Character;
using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

[Pool(typeof(BalatroCardPool))]
public class AncientJoker() : BalatroCard(0,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy), IRandomType
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4, ValueProp.Move),
        ..MakeCalculatedDamage(this.DynamicVars.Damage.IntValue, (card, target) => card.DynamicVars.Damage.BaseValue * (decimal) Math.Pow(1.5, PileType.Play.GetPile(card.Owner).Cards.Count(c => c.Type == CurrentType)) - 1M),
        new DisplayVar<AncientJoker>("type", card =>  ((IRandomType) card).GetTypeString())
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(this.DynamicVars.CalculatedDamage.Calculate(play.Target)).FromCard(this).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(2);
    }

    public CardType CurrentType { get; set; } = CardType.None;
    
}
