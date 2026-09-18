using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace Balatro.BalatroCode.Relics;

public class Shattered : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override async Task AfterActEntered()
    {
        if (Owner.RunState.TotalFloor < 2) await ShatterDeck();
    }

    private async Task ShatterDeck()
    {
        foreach (var card in PileType.Deck.GetPile(Owner).Cards.Where(c => c is not AscendersBane).ToList())
            card.RemoveFromCurrentPile();
        var results = new List<CardPileAddResult>();
        for (var i = 0; i < 20; ++i)
            results.Add(await CardPileCmd.Add(CardFactory.CreateForReward(Owner, 1, CardCreationOptions
                .ForNonCombatWithUniformOdds([Owner.Character.CardPool])
                .WithFlags(CardCreationFlags.NoRarityModification)).First().Card, PileType.Deck));
        foreach (var result in results)
        {
            CardCmd.PreviewCardPileAdd(result, style: CardPreviewStyle.MessyLayout);
            await Cmd.CustomScaledWait(0.1f, 0.2f);
        }

        await SaveManager.Instance.SaveRun(Owner.RunState.CurrentRoom);
    }
}