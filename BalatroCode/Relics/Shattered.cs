using Balatro.BalatroCode.Relics;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Relics;

public class Shattered() : BalatroRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;
    
    public override async Task AfterObtained()
    {
        this.Owner.RelicGrabBag.Remove<PandorasBox>();
        _ = TaskHelper.RunSafely(ShatterDeck());
    }
    
    private async Task ShatterDeck()
    {
        while (!LocalContext.IsMe(Owner))
            await Cmd.Wait(1);
        
        await CardPileCmd.RemoveFromDeck(PileType.Deck.GetPile(this.Owner).Cards.Where(c => c is not AscendersBane).ToList(), false);
        List<CardPileAddResult> results = new List<CardPileAddResult>();
        for (int i = 0; i < 20; ++i)
        {
            results.Add(await CardPileCmd.Add(CardFactory.CreateForReward(this.Owner, 1, CardCreationOptions
                        .ForNonCombatWithUniformOdds([this.Owner.Character.CardPool])
                        .WithFlags(CardCreationFlags.NoRarityModification)).First().Card, PileType.Deck));
        }
        foreach (CardPileAddResult result in results)
        {
            CardCmd.PreviewCardPileAdd(result, style: CardPreviewStyle.MessyLayout);
            await Cmd.CustomScaledWait(0.1f, 0.2f);
        }
    }
}