using Balatro.BalatroCode.Cards;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class Castle() : BalatroCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self), IRandomType
{
    private decimal _extraBlockFromDiscard;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5, ValueProp.Move),
        new("Increase", 2),
        new DisplayVar<Castle>("Type", card => ((IRandomType)card).GetTypeString())
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Increase"].UpgradeValueBy(1);
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        var block = DynamicVars.Block;
        block.BaseValue = block.BaseValue + _extraBlockFromDiscard;
    }

    public override Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Type == CurrentType)
        {
            var baseValue = card.DynamicVars["Increase"].BaseValue;
            BuffFromDiscard(baseValue);
        }

        return base.AfterCardDiscarded(choiceContext, card);
    }

    private void BuffFromDiscard(decimal extraBlock)
    {
        var block = DynamicVars.Block;
        block.BaseValue = block.BaseValue + extraBlock;
        _extraBlockFromDiscard += extraBlock;
    }

    public CardType CurrentType { get; set; } = CardType.None;
}