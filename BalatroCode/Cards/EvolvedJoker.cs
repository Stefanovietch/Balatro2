using Balatro.BalatroCode.Cards;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class EvolvedJoker() : BalatroCard(1,
    CardType.Skill, CardRarity.Ancient,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(20, ValueProp.Move),
        new BlockVar(20, ValueProp.Move),
        new GoldVar(20),
        new DisplayVar<EvolvedJoker>("ExtraText", e => 
            e.LastCardPlayedType() is (CardType.Power or CardType.Skill or CardType.Attack) 
                ? $"({e.LastCardPlayedType().ToString()})"
                : "")
    ];
    
    public override TargetType TargetType => LastCardPlayedType() == CardType.Attack ? TargetType.AnyEnemy : TargetType.Self;
    
    protected override bool ShouldGlowRedInternal => LastCardPlayedType() is not (CardType.Power or CardType.Skill or CardType.Attack);
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        switch (LastCardPlayedType())
        {
            case CardType.Power:
                await PlayerCmd.GainGold(DynamicVars.Gold.BaseValue, this.Owner);
                break;
            case CardType.Skill:
                await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
                break;
            case CardType.Attack:
                ArgumentNullException.ThrowIfNull(play.Target);
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
                    .Targeting(play.Target)
                    .WithHitFx("vfx/vfx_dramatic_stab")
                    .Execute(choiceContext);
                break;
        }
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(10);
        this.DynamicVars.Block.UpgradeValueBy(10);
        this.DynamicVars.Gold.UpgradeValueBy(10);
    }
    
    private CardType LastCardPlayedType()
    {
        if (this.IsCanonical) return CardType.None;
        return CombatManager.Instance.History.CardPlaysFinished.LastOrDefault(c => c.CardPlay.Card.Owner == Owner)
            ?.CardPlay.Card.Type ?? CardType.None;
    }
    
}