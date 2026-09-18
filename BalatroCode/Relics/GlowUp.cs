using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Relics;

public class GlowUp : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    public override bool HasUponPickupEffect => true;

    private List<CardModel> AncientCards =>
    [
        ModelDb.Card<Canio>(), ModelDb.Card<Chicot>(), ModelDb.Card<Perkeo>(),
        ModelDb.Card<Triboulet>(), ModelDb.Card<Yorick>()
    ];

    public override async Task AfterObtained()
    {
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.Add(
                Owner.RunState.CreateCard(Owner.PlayerRng.Rewards.NextItem(AncientCards) ?? ModelDb.Card<Perkeo>(),
                    Owner), PileType.Deck), 2f);
    }
}