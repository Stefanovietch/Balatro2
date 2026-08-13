using Balatro.BalatroCode.Powers;
using Balatro.BalatroCode.UI;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Powers;

public class TheHousePower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;
    
    public BlindType BlindType => BlindType.TheHouse;
    
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Character is Character.Balatro && player.PlayerCombatState?.TurnNumber == 1)
        {
            foreach (var card in PileType.Hand.GetPile(player).Cards)
            {
                await CardCmd.Afflict<BalatroHoused>(card, 3M);
            }
        }

        if (player.Character is Character.Balatro && player.PlayerCombatState?.TurnNumber > 1)
        {
            foreach (CardModel card in player.PlayerCombatState.AllCards.Where(c => c.Affliction is BalatroHoused))
                CardCmd.ClearAffliction(card);
        }
    }
    
    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        Decimal originalCost,
        out Decimal modifiedCost)
    {
        if (card.Affliction is not BalatroHoused)
        {
            modifiedCost = originalCost;
            return false;
        }
        modifiedCost = card.Affliction.Amount;
        return true;
    }
    
    public override Task AfterRemoved(Creature oldOwner)
    {
        var players = this.Owner.CombatState?.RunState.Players;
        if (players is null) return Task.CompletedTask;
        foreach (var player in players)
        {
            var playerPlayerCombatState = player.PlayerCombatState;
            if (playerPlayerCombatState is null || player.Character is not Character.Balatro) continue;
            foreach (CardModel card in playerPlayerCombatState.AllCards.Where(c => c.Affliction is BalatroHoused))
                CardCmd.ClearAffliction(card);
        }
        return Task.CompletedTask;
    }
}

public sealed class BalatroHoused : BalatroAfflictions
{
}