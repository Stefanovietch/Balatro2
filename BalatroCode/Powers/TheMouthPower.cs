using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Powers;

public class TheMouthPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    private Dictionary<Player, HashSet<CardType>> PlayedCardTypes { get; } = new();

    public override PowerStackType StackType =>
        PowerStackType.Single;
    
    public BlindType BlindType => BlindType.TheMouth;

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        PlayedCardTypes.Clear();
        return base.AfterSideTurnStart(side, participants, combatState);
    }

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner.Character is not Character.Balatro) return base.ShouldPlay(card, autoPlayType);
        
        if (PlayedCardTypes.TryGetValue(card.Owner, out var types) && !types.Contains(card.Type) && types.Count >= 2)
        {
            return false;
        }
        return base.ShouldPlay(card, autoPlayType);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Character is not Character.Balatro) return base.AfterCardPlayed(choiceContext, cardPlay);
            
        if (!PlayedCardTypes.TryGetValue(cardPlay.Card.Owner, out var types))
        {
            PlayedCardTypes[cardPlay.Card.Owner] = types = [];
        }
        
        if (types.Count < 2)
        {
            types.Add(cardPlay.Card.Type);
        }
        return base.AfterCardPlayed(choiceContext, cardPlay);
    }
}