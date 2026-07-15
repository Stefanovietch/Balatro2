using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Powers;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Powers;

public class LuckyCatPower() : BalatroPower, IChance
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DisplayVar<BusinessCard>("Numerator", card => card.GetNumerator(card.Owner).ToString()),
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (this.Owner.Player != player) return;
        if (this.RollChance(this.Owner.Player, 4))
        {
            await CreatureCmd.GainBlock(this.Owner, Amount, ValueProp.Move, null);
        }
        if (this.RollChance(this.Owner.Player, 4))
        {
            await CreatureCmd.Damage(choiceContext, this.CombatState.HittableEnemies, Amount, ValueProp.Unpowered, this.Owner, null);
        }
    }


}