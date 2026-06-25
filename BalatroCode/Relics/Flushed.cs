using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;

namespace Balatro.BalatroCode.Relics;

public class Flushed() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(2)
    ];

    private bool AnyPowersPlayedLastTurn;
    private bool AnyPowersPlayedThisTurn;
    
    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (this.Owner != cardPlay.Card.Owner || !CombatManager.Instance.IsInProgress || cardPlay.Card.Type != CardType.Power || this.AnyPowersPlayedThisTurn)
            return Task.CompletedTask;
        this.Status = RelicStatus.Normal;
        this.AnyPowersPlayedThisTurn = true;
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != this.Owner.Creature.Side)
            return Task.CompletedTask;
        this.AnyPowersPlayedLastTurn = this.AnyPowersPlayedThisTurn;
        this.AnyPowersPlayedThisTurn = false;
        return Task.CompletedTask;
    }
    
    public override async Task AfterEnergyReset(Player player)
    {
        if (player != this.Owner) return;
        this.Status = RelicStatus.Active;
        if (this.Owner.Creature.CombatState?.RoundNumber <= 1)
            return;
        if (!this.AnyPowersPlayedLastTurn)
        {
            this.Flash();
            await PlayerCmd.GainEnergy(this.DynamicVars.Energy.BaseValue, this.Owner);
        }
        this.AnyPowersPlayedLastTurn = false;
        this.AnyPowersPlayedThisTurn = false;
    }
    
    public override Task AfterCombatEnd(CombatRoom _)
    {
        this.Status = RelicStatus.Normal;
        this.AnyPowersPlayedLastTurn = false;
        this.AnyPowersPlayedThisTurn = false;
        return Task.CompletedTask;
    }

}