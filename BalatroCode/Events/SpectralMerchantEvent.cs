using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Events;

public class SpectralMerchantEvent : CustomEventModel
{
    private CardModel? _randomCardToLoseCommon;

    private CardModel? _randomCardToLoseRare;
    private CardModel? _randomCardToLoseUncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("RandomCardRare"),
        new StringVar("RandomCardUncommon"),
        new StringVar("RandomCardCommon")
    ];

    public override ActModel[] Acts =>
    [
        ModelDb.Act<Underdocks>(),
        ModelDb.Act<Overgrowth>()
    ];

    public override string CustomInitialPortraitPath => "res://Balatro/images/events/spectral_merchant.png";


    public override bool IsAllowed(IRunState runState)
    {
        return runState.TotalFloor > 6 &&
               runState.Players.All(p => p.Deck.Cards.Count(c => c.IsRemovable && c.Rarity != CardRarity.Basic) >= 3) &&
               runState.Players.Any(p => p.Character is Character.Balatro);
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            Option(Approach),
            Option(Leave)
        ];
    }

    public async Task Approach()
    {
        SetRandomCards();
        SetEventState(L10NLookup("BALATRO-SPECTRAL_MERCHANT_EVENT.pages.APPROACH.description"), [
            Option(Grim, [HoverTipFactory.FromCard(_randomCardToLoseRare!)], "APPROACH"),
            Option(Familiar, [HoverTipFactory.FromCard(_randomCardToLoseUncommon!)], "APPROACH"),
            Option(Incantation, [HoverTipFactory.FromCard(_randomCardToLoseCommon!)], "APPROACH")
        ]);
    }

    public async Task Grim()
    {
        await CardPileCmd.RemoveFromDeck(_randomCardToLoseRare!);
        var options =
            new CardCreationOptions([Owner!.Character.CardPool], CardCreationSource.Other, CardRarityOddsType.Uniform,
                (Func<CardModel, bool>)(c => c.Rarity == CardRarity.Rare)).WithFlags(CardCreationFlags.NoUpgradeRoll);
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.Add(CardFactory.CreateForReward(Owner, 1, options).FirstOrDefault()!.Card,
                PileType.Deck));
        SetEventFinished(L10NLookup("BALATRO-SPECTRAL_MERCHANT_EVENT.pages.FINISHED.description"));
    }

    public async Task Familiar()
    {
        await CardPileCmd.RemoveFromDeck(_randomCardToLoseUncommon!);
        for (var i = 0; i < 2; i++)
        {
            var options =
                new CardCreationOptions([Owner!.Character.CardPool], CardCreationSource.Other,
                        CardRarityOddsType.Uniform, (Func<CardModel, bool>)(c => c.Rarity == CardRarity.Uncommon))
                    .WithFlags(CardCreationFlags.NoUpgradeRoll);
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.Add(CardFactory.CreateForReward(Owner, 1, options).FirstOrDefault()!.Card,
                    PileType.Deck));
        }

        SetEventFinished(L10NLookup("BALATRO-SPECTRAL_MERCHANT_EVENT.pages.FINISHED.description"));
    }

    public async Task Incantation()
    {
        await CardPileCmd.RemoveFromDeck(_randomCardToLoseCommon!);
        for (var i = 0; i < 3; i++)
        {
            var options =
                new CardCreationOptions([Owner!.Character.CardPool], CardCreationSource.Other,
                        CardRarityOddsType.Uniform, (Func<CardModel, bool>)(c => c.Rarity == CardRarity.Common))
                    .WithFlags(CardCreationFlags.NoUpgradeRoll);
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.Add(CardFactory.CreateForReward(Owner, 1, options).FirstOrDefault()!.Card,
                    PileType.Deck));
        }

        SetEventFinished(L10NLookup("BALATRO-SPECTRAL_MERCHANT_EVENT.pages.FINISHED.description"));
    }

    private async Task Leave()
    {
        SetEventFinished(L10NLookup("BALATRO-SPECTRAL_MERCHANT_EVENT.pages.LEAVE.description"));
    }


    private void SetRandomCards()
    {
        var list = Owner!.Deck.Cards.Where(c => c.IsRemovable && c.Rarity != CardRarity.Basic).ToList();
        var randomCards = list.TakeRandom(3, Rng).ToList();
        _randomCardToLoseRare = randomCards[0];
        _randomCardToLoseUncommon = randomCards[1];
        _randomCardToLoseCommon = randomCards[2];
        ((StringVar)DynamicVars["RandomCardRare"]).StringValue = _randomCardToLoseRare.Title;
        ((StringVar)DynamicVars["RandomCardUncommon"]).StringValue = _randomCardToLoseUncommon.Title;
        ((StringVar)DynamicVars["RandomCardCommon"]).StringValue = _randomCardToLoseCommon.Title;
    }
}