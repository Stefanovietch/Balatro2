using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Powers;

public class TheFlintPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;
    
    public BlindType BlindType => BlindType.TheFlint;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Character is Character.Balatro)
        {
            var vulnerablePower = new VulnerablePower();
            var prop = vulnerablePower.GetType().GetProperty(nameof(vulnerablePower.Type));
            prop?.SetValue(vulnerablePower, PowerType.Buff);
            var prop2 = vulnerablePower.GetType().GetProperty(nameof(vulnerablePower.StackType));
            prop2?.SetValue(vulnerablePower, PowerStackType.Single);
            PowerCmd.Apply(choiceContext, vulnerablePower, player.Creature, 1, null, null);
            
            var weakPower = new WeakPower();
            var prop3 = weakPower.GetType().GetProperty(nameof(weakPower.Type));
            prop3?.SetValue(weakPower, PowerType.Buff);
            var prop4 = weakPower.GetType().GetProperty(nameof(weakPower.StackType));
            prop4?.SetValue(weakPower, PowerStackType.Single);
            PowerCmd.Apply(choiceContext, weakPower, player.Creature, 1, null, null);
        }
        return base.AfterPlayerTurnStart(choiceContext, player);
    }
}