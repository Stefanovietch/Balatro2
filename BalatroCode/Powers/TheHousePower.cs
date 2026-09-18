using Balatro.BalatroCode.Afflictions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Powers;

public class TheHousePower : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public BlindType BlindType => BlindType.TheHouse;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        return QueueHouseAffliction();
    }

    private async Task QueueHouseAffliction()
    {
        await Task.Yield();

        var players = Owner.CombatState?.Players;
        if (players is null)
            return;

        foreach (var player in players)
        {
            if (player.Character is not Character.Balatro)
                continue;

            var cards = PileType.Hand.GetPile(player).Cards.ToList();

            foreach (var card in cards)
                await CardCmd.Afflict<Housed>(card, 3M);
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Character is Character.Balatro && player.PlayerCombatState?.TurnNumber > 1)
            foreach (var card in player.PlayerCombatState.AllCards.Where(c => c.Affliction is Housed))
                CardCmd.ClearAffliction(card);
    }

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        if (card.Affliction is not Housed)
        {
            modifiedCost = originalCost;
            return false;
        }

        modifiedCost = card.Affliction.Amount;
        return true;
    }

    public override async Task AfterDeath(
        PlayerChoiceContext choiceContext,
        Creature creature,
        bool wasRemovalPrevented,
        float deathAnimLength)
    {
        if (wasRemovalPrevented || creature != Owner) return;
        await PowerCmd.Remove(this);
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        var players = Owner.CombatState?.Players;
        if (players is null) return Task.CompletedTask;
        foreach (var player in players)
        {
            var playerPlayerCombatState = player.PlayerCombatState;
            if (playerPlayerCombatState is null || player.Character is not Character.Balatro) continue;
            foreach (var card in playerPlayerCombatState.AllCards.Where(c => c.Affliction is Housed))
                CardCmd.ClearAffliction(card);
        }

        return Task.CompletedTask;
    }
}