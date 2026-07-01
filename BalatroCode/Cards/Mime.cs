using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Cards;

public class Mime() : BalatroCard(3,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(this.CombatState);
        var cards = PileType.Hand.GetPile(this.Owner);
        cards.RandomizeOrderInternal(this.Owner, this.Owner.RunState.Rng.CombatCardSelection, (CombatState) this.CombatState);
        foreach (var card in cards.Cards.ToList())
        {
            await CardCmd.AutoPlay(choiceContext, card, null);
        }
    }

    protected override void OnUpgrade()
    {
        this.EnergyCost.UpgradeBy(-1);
    }
}
