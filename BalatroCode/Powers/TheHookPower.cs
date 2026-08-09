using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Powers;

public class TheHookPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;
    
    public BlindType BlindType => BlindType.TheHook;
    
    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        var enumerable = participants.ToList();
        if (!enumerable.Contains(this.Owner) || side != CombatSide.Player) return;
        foreach (var p in enumerable.Where(c => c is { IsPlayer: true, IsAlive: true, Player.Character: Character.Balatro }))
        {
            if (p.Player == null) continue;
            List<CardModel> cards = (await CardSelectCmd.FromHandForDiscard(choiceContext, p.Player,
                new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 2), null, this)).ToList();
            if (cards.Count != 0) await CardCmd.Discard(choiceContext, cards);
        }
    }

}