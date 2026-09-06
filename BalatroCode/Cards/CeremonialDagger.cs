using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class CeremonialDagger() : BalatroCard(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    private int _currentDmg = 9;
    private int _increasedDmg;

    [SavedProperty]
    public int CurrentDamage
    {
        get => _currentDmg;
        set
        {
            AssertMutable();
            _currentDmg = value;
            DynamicVars.Damage.BaseValue = _currentDmg;
        }
    }

    [SavedProperty]
    public int IncreasedDamage
    {
        get => _increasedDmg;
        set
        {
            AssertMutable();
            _increasedDmg = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(CurrentDamage, ValueProp.Move)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        CardModel? card;
        if (IsUpgraded)
        {
            var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
            card = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this)).FirstOrDefault();
        }
        else
        {
            var pile = PileType.Hand.GetPile(Owner);
            card = Owner.RunState.Rng.CombatCardSelection.NextItem(pile.Cards);
        }

        if (card != null)
        {
            await CardCmd.Exhaust(choiceContext, card);
            var intValue = 2 * card.EnergyCost.GetAmountToSpend();
            BuffFromExhaust(intValue);
            if (DeckVersion is CeremonialDagger deckVersion) deckVersion.BuffFromExhaust(intValue);
        }
        
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
    }

    private void BuffFromExhaust(int extradmg)
    {
        IncreasedDamage += extradmg;
        UpdateDmg();
    }

    private void UpdateDmg()
    {
        CurrentDamage = 9 + IncreasedDamage;
    }
}