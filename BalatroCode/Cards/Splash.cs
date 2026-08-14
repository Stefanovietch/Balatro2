using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public class Splash() : BalatroCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("Cleanse", 1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var powersToCleanse = Owner.Creature.Powers.Where(p => p.Type == PowerType.Debuff)
            .TakeRandom(DynamicVars["Cleanse"].IntValue, Owner.RunState.Rng.CombatTargets).ToList();
        if (powersToCleanse.Count == 0) return;
        foreach (var power in powersToCleanse) await PowerCmd.Remove(power);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Cleanse"].UpgradeValueBy(1);
    }
}