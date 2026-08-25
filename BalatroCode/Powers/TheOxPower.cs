using Balatro.BalatroCode.Afflictions;
using Balatro.BalatroCode.Powers;
using Balatro.BalatroCode.UI;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Balatro.BalatroCode.Powers;

public class TheOxPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public BlindType BlindType => BlindType.TheOx;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        var enumerable = participants.ToList();
        if (side != CombatSide.Player) return;
        foreach (var p in enumerable.Where(c => c is
                     { IsPlayer: true, IsAlive: true, Player.Character: Character.Balatro }))
        {
            if (p.Player == null || p.Player.PlayerCombatState == null) continue;
            foreach (var oxed in p.Player.PlayerCombatState.AllCards.Where(c => c.Affliction is Oxed))
                CardCmd.ClearAffliction(oxed);
            
            var card = PileType.Hand.GetPile(p.Player).Cards.TakeRandom(1, p.Player.RunState.Rng.CombatCardSelection)
                .FirstOrDefault();
            if (card == null) continue;
            await CardCmd.AfflictAndPreview<Oxed>([card], 1, CardPreviewStyle.None);
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Affliction is Oxed)
        {
            await PlayerCmd.SetGold(0, cardPlay.Card.Owner);
            CardCmd.ClearAffliction(cardPlay.Card);
        }
    }
    
    public override async Task AfterDeath(
        PlayerChoiceContext choiceContext,
        Creature creature,
        bool wasRemovalPrevented,
        float deathAnimLength)
    {
        if (wasRemovalPrevented || creature != this.Owner) return;
        await PowerCmd.Remove(this);
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        var players = Owner.CombatState?.RunState.Players;
        if (players is null) return Task.CompletedTask;
        foreach (var player in players)
        {
            var playerPlayerCombatState = player.PlayerCombatState;
            if (playerPlayerCombatState is null || player.Character is not Character.Balatro) continue;
            foreach (var card in playerPlayerCombatState.AllCards.Where(c => c.Affliction is Oxed))
                CardCmd.ClearAffliction(card);
        }

        return Task.CompletedTask;
    }
}