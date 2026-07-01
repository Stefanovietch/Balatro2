using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class StoneCard() : BalatroCard(1,
    CardType.Skill, CardRarity.Token,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(12, ValueProp.Move)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.Block.BaseValue, ValueProp.Move , play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(4);
    }
    
    public static async Task<IEnumerable<CardModel>> CreateInHand(
        Player owner,
        int count,
        ICombatState combatState)
    {
        if (count == 0)
            return Array.Empty<CardModel>();
        if (CombatManager.Instance.IsOverOrEnding)
            return Array.Empty<CardModel>();
        List<CardModel> stoneCards = new List<CardModel>();
        for (int index = 0; index < count; ++index)
            stoneCards.Add(combatState.CreateCard<Shiv>(owner));
        IReadOnlyList<CardPileAddResult> combat = await CardPileCmd.AddGeneratedCardsToCombat(stoneCards, PileType.Hand, owner);
        return stoneCards;
    }
}