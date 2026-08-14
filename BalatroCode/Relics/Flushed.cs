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

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(2)
    ];

    private bool AnyPowersPlayedLastTurn;
    private bool AnyPowersPlayedThisTurn;

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (Owner != cardPlay.Card.Owner || !CombatManager.Instance.IsInProgress ||
            cardPlay.Card.Type != CardType.Power || AnyPowersPlayedThisTurn)
            return Task.CompletedTask;
        Status = RelicStatus.Normal;
        AnyPowersPlayedThisTurn = true;
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Creature.Side)
            return Task.CompletedTask;
        AnyPowersPlayedLastTurn = AnyPowersPlayedThisTurn;
        AnyPowersPlayedThisTurn = false;
        return Task.CompletedTask;
    }

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner) return;
        Status = RelicStatus.Active;
        if (Owner.Creature.CombatState?.RoundNumber <= 1)
            return;
        if (!AnyPowersPlayedLastTurn)
        {
            Flash();
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        }

        AnyPowersPlayedLastTurn = false;
        AnyPowersPlayedThisTurn = false;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
        AnyPowersPlayedLastTurn = false;
        AnyPowersPlayedThisTurn = false;
        return Task.CompletedTask;
    }
}