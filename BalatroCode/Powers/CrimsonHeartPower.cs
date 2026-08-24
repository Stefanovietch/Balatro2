using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Balatro.BalatroCode.Powers;

public class CrimsonHeartPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public BlindType BlindType => BlindType.CrimsonHeart;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Character is not Character.Balatro) return;
        var card = PileType.Hand.GetPile(player).Cards.TakeRandom(1, player.RunState.Rng.CombatCardSelection)
            .FirstOrDefault();
        if (card == null) return;
        await CardCmd.Discard(choiceContext, card);
    }
}