using Balatro.BalatroCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Relics;

public class GlowUp() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    public override async Task AfterObtained()
    {
        List<CardPileAddResult> results = new List<CardPileAddResult>();
        results.Add(await CardPileCmd.Add(this.Owner.RunState.CreateCard(ModelDb.Card<SeekerStrike>(), this.Owner), PileType.Deck));
        CardCmd.PreviewCardPileAdd(results, 2f);
    }
}