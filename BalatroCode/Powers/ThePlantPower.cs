using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Powers;

public class ThePlantPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;
    
    public BlindType BlindType => BlindType.ThePlant;
    
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var cardModels = this.Owner.Player?.PlayerCombatState?.AllCards;
        if (cardModels != null)
            foreach (CardModel card in cardModels)
            {
                if (card.Owner.Character is not Character.Balatro || card.Type != CardType.Power) continue;
                await CardCmd.Afflict<Planted>(card, 1);
            }
    }
    public override async Task AfterCardEnteredCombat(CardModel card)
    {
        if (card.Owner.Character is not Character.Balatro || card.Type != CardType.Power) return;
        await CardCmd.Afflict<Planted>(card, 1);
    }
    
    public override bool TryModifyKeywordsInCombat(CardModel card, ISet<CardKeyword> keywords)
    {
        return card.Owner == this.Owner.Player && card.Affliction is Planted && keywords.Add(CardKeyword.Ethereal);
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        var playerPlayerCombatState = this.Owner.Player?.PlayerCombatState;
        if (playerPlayerCombatState is not null)
            foreach (CardModel card in playerPlayerCombatState.AllCards.Where(c => c.Affliction is Planted))
                CardCmd.ClearAffliction(card);
        return Task.CompletedTask;
    }
}

public sealed class Planted : AfflictionModel
{
}