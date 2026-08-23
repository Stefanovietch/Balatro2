using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace Balatro.BalatroCode.Relics;

public class Flushed() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    
    private bool _anyPowersPlayedLastTurn;
    private bool _anyPowersPlayedThisTurn;

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (Owner != cardPlay.Card.Owner || !CombatManager.Instance.IsInProgress ||
            cardPlay.Card.Type != CardType.Power || _anyPowersPlayedThisTurn)
            return Task.CompletedTask;
        Status = RelicStatus.Normal;
        _anyPowersPlayedThisTurn = true;
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Creature.Side)
            return Task.CompletedTask;
        _anyPowersPlayedLastTurn = _anyPowersPlayedThisTurn;
        _anyPowersPlayedThisTurn = false;
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        Status = RelicStatus.Active;
        if (Owner.Creature.CombatState?.RoundNumber <= 1)
            return;
        if (!_anyPowersPlayedLastTurn)
        {
            Flash();
            await PowerCmd.Apply<DexterityPower>(choiceContext, this.Owner.Creature, 1M, null, null);
            await PowerCmd.Apply<StrengthPower>(choiceContext, this.Owner.Creature, 1M, null, null);
        }

        _anyPowersPlayedLastTurn = false;
        _anyPowersPlayedThisTurn = false;
    }
    

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
        _anyPowersPlayedLastTurn = false;
        _anyPowersPlayedThisTurn = false;
        return Task.CompletedTask;
    }
}