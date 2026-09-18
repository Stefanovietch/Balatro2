using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Balatro.BalatroCode.Enchantments;

public class GoldSeal : CustomEnchantmentModel
{
    public override bool ShowAmount => false;

    protected override string CustomIconPath => "Balatro/images/enchantments/gold_seal.png";

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (cardPlay?.Card.Owner != null) await PlayerCmd.GainGold(Amount, cardPlay.Card.Owner);
    }

    public override bool CanEnchantCardType(CardType cardType)
    {
        return true;
    }
}