using Balatro.BalatroCode.Afflictions;
using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Powers;
using Balatro.BalatroCode.UI;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Powers;

public class TheWheelPower() : BalatroPower, IBlindPower, IChance
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public BlindType BlindType => BlindType.TheWheel;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DisplayVar<TheWheelPower>("Numerator", power => power.GetNumerator(power.Owner.Player).ToString())
    ];

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Character is Character.Balatro)
            if (this.RollChance(card.Owner, 7))
                await CardCmd.Afflict<Wheeled>(card, 3M);
    }
    
    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        if (card.Affliction is not Wheeled)
        {
            modifiedCost = originalCost;
            return false;
        }

        modifiedCost = card.Affliction.Amount;
        return true;
    }
    
    public override Task AfterRemoved(Creature oldOwner)
    {
        var players = Owner.CombatState?.RunState.Players;
        if (players is null) return Task.CompletedTask;
        foreach (var player in players)
        {
            var playerPlayerCombatState = player.PlayerCombatState;
            if (playerPlayerCombatState is null || player.Character is not Character.Balatro) continue;
            foreach (var card in playerPlayerCombatState.AllCards.Where(c => c.Affliction is Wheeled))
                CardCmd.ClearAffliction(card);
        }

        return Task.CompletedTask;
    }
}