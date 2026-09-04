using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Character;
using BaseLib.Cards.Variables;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class GrosMichel() : BalatroCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AllEnemies), IChance
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("Chance", 6),
        new DisplayVar<GrosMichel>("Numerator", card => card
            .GetNumerator(card.IsCanonical ? null : card.Owner).ToString()),
        new DamageVar(6, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_giant_horizontal_slash")
            .Execute(choiceContext);
        if (this.RollChance(Owner, DynamicVars["Chance"].IntValue))
        {
            await CardPileCmd.RemoveFromCombat(this);
            if (DeckVersion is not GrosMichel deckVersion)
                return;
            await CardPileCmd.RemoveFromDeck(deckVersion);
            Character.Balatro.GrosMichelExtinct.Set(Owner,true);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}