using Balatro.BalatroCode.Powers;
using BaseLib.Hooks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Balatro.BalatroCode.Powers;

public class TheWallPower() : BalatroPower, IBlindPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public BlindType BlindType => BlindType.TheWall;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var hpDiff = Owner.MaxHp - Owner.CurrentHp;
        Owner.SetMaxHpInternal(Owner.MaxHp * 2);
        Owner.SetCurrentHpInternal(Owner.MaxHp - hpDiff);
        if (Owner.Monster is TerrorEel)
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), Owner.GetPower<ShriekPower>()!, 
                AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 75 , 70), 
                null, null, true);
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        if (this.Owner.IsDead) return;
        var hpDiff = Owner.MaxHp - Owner.CurrentHp;
        Owner.SetMaxHpInternal(Owner.MaxHp / 2M);
        var newHp = Owner.MaxHp - hpDiff < 0 ? 0 : Owner.MaxHp - hpDiff;
        Owner.SetCurrentHpInternal(newHp);
        if (Owner.Monster is TerrorEel) await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), Owner.GetPower<ShriekPower>()!, 
            AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 75 , 70), 
            null, null, true);
    }
}