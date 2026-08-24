using Balatro.BalatroCode.Cards;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class MadJoker() : BalatroCard(1,
    CardType.Attack, CardRarity.Basic,
    TargetType.AnyEnemy), ITranscendenceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12, ValueProp.Move)
    ];
    
    public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<EvolvedJoker>();


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        if (!LastCardIsAttack()) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }

    protected override bool ShouldGlowRedInternal => !LastCardIsAttack();

    private bool LastCardIsAttack()
    {
        var yourCardPlayed =
            CombatManager.Instance.History.CardPlaysFinished.LastOrDefault(c => c.CardPlay.Card.Owner == Owner);
        return yourCardPlayed?.CardPlay.Card.Type == CardType.Attack;
    }
}