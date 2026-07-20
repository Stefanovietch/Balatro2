using Balatro.BalatroCode.Cards;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class GrosMichel() : BalatroCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AllEnemies), IChance
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Chance", 6),
        new DisplayVar<GrosMichel>("Numerator", card => card
            .GetNumerator(card.IsCanonical ? null : card.Owner).ToString()),
        new DamageVar(6, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(this.CombatState);
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard(this)
            .TargetingAllOpponents(this.CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (this.RollChance(this.Owner, this.DynamicVars["Chance"].IntValue))
        {
            await CardPileCmd.RemoveFromCombat(this);
            if (this.DeckVersion is not Cavendish deckVersion)
                return;
            await CardPileCmd.RemoveFromDeck(deckVersion);
        }
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(4);
    }
}
