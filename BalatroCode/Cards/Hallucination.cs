using Balatro.BalatroCode.Cards;
using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Cards;

public class Hallucination() : BalatroCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.AnyEnemy), IChance
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("Chance", 6),
        new DisplayVar<Hallucination>("Numerator", card => card
            .GetNumerator(card.IsCanonical ? null : card.Owner).ToString()),
        new PowerVar<WeakPower>(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await PowerCmd.Apply<WeakPower>(choiceContext, play.Target, DynamicVars.Power<WeakPower>().BaseValue,
            Owner.Creature, this);
        if (this.RollChance(Owner, DynamicVars["Chance"].IntValue))
            if (Owner.RunState.CurrentRoom is CombatRoom room)
            {
                var potionReward = new PotionReward(Owner);
                potionReward.Populate();
                room.AddExtraReward(Owner, potionReward);
            }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<WeakPower>().UpgradeValueBy(1);
    }
}