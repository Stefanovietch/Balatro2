using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Cards;

public class MarbleJoker() : BalatroCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(this.CombatState);
        IEnumerable<CardModel> inHand = await StoneCard.CreateInHand(this.Owner, 1, this.CombatState);
        if (!this.IsUpgraded) return;
        foreach (CardModel card in inHand)
            CardCmd.Upgrade(card);
    }

    protected override void OnUpgrade()
    {

    }
}
