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
        this.Flash();
        int cardsToUpgrade = this.Owner.Deck.Cards.Count / 2;
        List<CardModel> list = PileType.Draw.GetPile(this.Owner).Cards.Where(c => c.IsUpgradable).ToList().StableShuffle(this.Owner.RunState.Rng.CombatCardSelection).Take(cardsToUpgrade).ToList();
        CardCmd.Upgrade(list, CardPreviewStyle.HorizontalLayout);
        CardCmd.Preview(list);
        await Cmd.CustomScaledWait(0.5f, 1f);
    }
}