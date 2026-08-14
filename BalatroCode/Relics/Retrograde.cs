using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rooms;

namespace Balatro.BalatroCode.Relics;

public class Retrograde() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is CombatRoom))
            return;
        Flash();
        var cardsToUpgrade = Owner.Deck.Cards.Count / 2;
        var list = PileType.Draw.GetPile(Owner).Cards.Where(c => c.IsUpgradable).ToList()
            .StableShuffle(Owner.RunState.Rng.CombatCardSelection).Take(cardsToUpgrade).ToList();
        CardCmd.Upgrade(list, CardPreviewStyle.HorizontalLayout);
        CardCmd.Preview(list);
        await Cmd.CustomScaledWait(0.5f, 1f);
    }
}