using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Balatro.BalatroCode.Cards;

public class SockAndBuskin() : BalatroCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        await Hook.AfterPlayerTurnStart(CombatState, choiceContext, Owner);
        
        await Hook.BeforeSideTurnStart(CombatState, CombatSide.Player, [Owner.Creature]);
        await Hook.AfterSideTurnStart(CombatState, CombatSide.Player, [Owner.Creature]);

        await Hook.BeforeSideTurnEnd(CombatState, CombatSide.Player, [Owner.Creature]);
        await Hook.AfterSideTurnEnd(CombatState, CombatSide.Player, [Owner.Creature]);
        
        Decimal handDraw = Hook.ModifyHandDraw(CombatState, Owner, 0M, out _);
        await CardPileCmd.Draw(choiceContext, handDraw, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}