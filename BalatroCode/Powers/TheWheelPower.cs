using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Powers;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Entities.Cards;
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
        new DisplayVar<TheWheelPower>("Numerator", power => power.GetNumerator(power.Owner.Player).ToString()),
    ];

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Character is Character.Balatro)
        {
            if (this.RollChance(card.Owner, 7))
            {
                card.EnergyCost.SetThisTurnOrUntilPlayed(3);
            }
        }
        return base.AfterCardDrawn(choiceContext, card, fromHandDraw);
    }
}