using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Odds;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Cards;

public class OopsAll6s() : BalatroCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<OopsAll6sPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        if (IsUpgraded && Owner.RunState.CurrentRoom is CombatRoom room)
        {
            if (!Owner.PlayerOdds.PotionReward.Roll(Owner, RunManager.Instance.AscensionManager, room.RoomType)) return;
            var potionReward = new PotionReward(Owner);
            potionReward.Populate();
            room.AddExtraReward(Owner, potionReward);
        }
    }
}