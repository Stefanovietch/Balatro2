using Balatro.BalatroCode.Cards;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Cards;

public class CleverJoker() : BalatroCard(1,
    CardType.Skill, CardRarity.Basic,
    TargetType.Self), ITranscendenceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(10, ValueProp.Move)
    ];
    
    public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<EvolvedJoker>();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (!LastCardIsSkill()) return;
        var num = await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4);
    }

    protected override bool ShouldGlowRedInternal => !LastCardIsSkill();

    private bool LastCardIsSkill()
    {
        var yourCardPlayed = CombatManager.Instance.History.CardPlaysFinished.LastOrDefault(c =>
            c.HappenedThisTurn(CombatState) && c.CardPlay.Card.Owner == Owner);
        return yourCardPlayed?.CardPlay.Card.Type == CardType.Skill;
    }
}