using Balatro.BalatroCode.Afflictions;
using Balatro.BalatroCode.Powers;
using Balatro.BalatroCode.UI;
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
        var players = Owner.CombatState?.RunState.Players;
        if (players is null) return;
        foreach (var player in players)
        {
            var playerPlayerCombatState = player.PlayerCombatState;
            if (playerPlayerCombatState is null || player.Character is not Character.Balatro) continue;
            foreach (var card in playerPlayerCombatState.AllCards)
            {
                if (card.Type != CardType.Power) continue;
                await CardCmd.Afflict<Planted>(card, 1M);
            }
        }
    }

    public override async Task AfterCardEnteredCombat(CardModel card)
    {
        if (card.Owner.Character is not Character.Balatro || card.Type != CardType.Power) return;
        await CardCmd.Afflict<Planted>(card, 1);
    }

    public override bool TryModifyKeywordsInCombat(CardModel card, ISet<CardKeyword> keywords)
    {
        return card.Affliction is Planted && keywords.Add(CardKeyword.Ethereal);
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        var players = Owner.CombatState?.RunState.Players;
        if (players is null) return Task.CompletedTask;
        foreach (var player in players)
        {
            var playerPlayerCombatState = player.PlayerCombatState;
            if (playerPlayerCombatState is null) continue;
            foreach (var card in playerPlayerCombatState.AllCards.Where(c => c.Affliction is Planted))
                CardCmd.ClearAffliction(card);
        }

        return Task.CompletedTask;
    }
}