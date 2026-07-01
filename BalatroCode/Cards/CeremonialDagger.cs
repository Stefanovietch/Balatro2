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

public class CeremonialDagger() : BalatroCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.Self)
{
    private int _currentDmg = 7;
    private int _increasedDmg;
    
    [SavedProperty]
    public int CurrentDamage
    {
        get => this._currentDmg;
        set
        {
            this.AssertMutable();
            this._currentDmg = value;
            this.DynamicVars.Damage.BaseValue = this._currentDmg;
        }
    }

    [SavedProperty]
    public int IncreasedDamage
    {
        get => this._increasedDmg;
        set
        {
            this.AssertMutable();
            this._increasedDmg = value;
        }
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(this.CurrentDamage, ValueProp.Move),
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);

        CardModel? card;
        if (this.IsUpgraded)
        {
            CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
            card = (await CardSelectCmd.FromHand(choiceContext, this.Owner, prefs, null, this)).FirstOrDefault();
            if (card == null)
                return;
            await CardCmd.Exhaust(choiceContext, card);
        }
        else
        {
            CardPile pile = PileType.Hand.GetPile(this.Owner);
            card = this.Owner.RunState.Rng.CombatCardSelection.NextItem(pile.Cards);
            if (card == null)
                return;
            await CardCmd.Exhaust(choiceContext, card);
        }

        int intValue;
        if (card.HasStarCostX) intValue = card.ResolveEnergyXValue();
        else intValue = card.EnergyCost.Canonical;
        this.BuffFromExhaust(intValue * 2);
        if (!(this.DeckVersion is CeremonialDagger deckVersion))
            return;
        deckVersion.BuffFromExhaust(intValue);
    }

    protected override void OnUpgrade()
    {

    }
    
    private void BuffFromExhaust(int extradmg)
    {
        this.IncreasedDamage += extradmg;
        this.UpdateDmg();
    }

    private void UpdateDmg() => this.CurrentDamage = 7 + this.IncreasedDamage;
}
