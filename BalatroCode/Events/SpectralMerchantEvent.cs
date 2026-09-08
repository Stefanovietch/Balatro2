using Balatro.BalatroCode.Extensions;
using Balatro.BalatroCode.Relics;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Events;

public class SpectralMerchantEvent() : CustomEventModel()
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("RandomCardRare"),
        new StringVar("RandomCardUncommon"),
        new StringVar("RandomCardCommon")
    ];
    
    private CardModel? _randomCardToLoseRare;
    private CardModel? _randomCardToLoseUncommon;
    private CardModel? _randomCardToLoseCommon;
    
    
    public override bool IsAllowed(IRunState runState)
    {
        return runState.TotalFloor > 6 && runState.Players.All(p => p.Deck.Cards.Count(c => c.IsRemovable && c.Rarity != CardRarity.Basic) >= 3) && runState.Players.Any(p => p.Character is Character.Balatro);
    }
    
    public override ActModel[] Acts =>
    [
        ModelDb.Act<Underdocks>(),
        ModelDb.Act<Overgrowth>()
    ];
    
    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
        [
            Option(Approach),
            Option(Leave)
        ];
    
    public async Task Approach()
    {
        SetRandomCards();
        this.SetEventState(this.L10NLookup("BALATRO-SPECTRAL_MERCHANT_EVENT.pages.APPROACH.description"), [
            Option(Grim, [HoverTipFactory.FromCard(this._randomCardToLoseRare!)], "APPROACH"),
            Option(Familiar, [HoverTipFactory.FromCard(this._randomCardToLoseUncommon!)], "APPROACH"),
            Option(Incantation, [HoverTipFactory.FromCard(this._randomCardToLoseCommon!)], "APPROACH")
        ]);
    }

    public async Task Grim()
    {
        await CardPileCmd.RemoveFromDeck(_randomCardToLoseRare!);
        CardCreationOptions options = new CardCreationOptions([Owner!.Character.CardPool], CardCreationSource.Other, CardRarityOddsType.Uniform, (Func<CardModel, bool>) (c => c.Rarity == CardRarity.Rare)).WithFlags(CardCreationFlags.NoUpgradeRoll);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(CardFactory.CreateForReward(Owner,1, options).FirstOrDefault()!.Card, PileType.Deck));
        this.SetEventFinished(this.L10NLookup("BALATRO-SPECTRAL_MERCHANT_EVENT.pages.FINISHED.description"));
    }
    
    public async Task Familiar()
    {
        await CardPileCmd.RemoveFromDeck(_randomCardToLoseUncommon!);
        for (int i = 0; i < 2; i++)
        {
            CardCreationOptions options = new CardCreationOptions([Owner!.Character.CardPool], CardCreationSource.Other, CardRarityOddsType.Uniform, (Func<CardModel, bool>) (c => c.Rarity == CardRarity.Uncommon)).WithFlags(CardCreationFlags.NoUpgradeRoll);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(CardFactory.CreateForReward(Owner,1, options).FirstOrDefault()!.Card, PileType.Deck));
        }
        this.SetEventFinished(this.L10NLookup("BALATRO-SPECTRAL_MERCHANT_EVENT.pages.FINISHED.description"));
    }
    
    public async Task Incantation()
    {
        await CardPileCmd.RemoveFromDeck(_randomCardToLoseCommon!);
        for (int i = 0; i < 3; i++)
        {
            CardCreationOptions options = new CardCreationOptions([Owner!.Character.CardPool], CardCreationSource.Other, CardRarityOddsType.Uniform, (Func<CardModel, bool>) (c => c.Rarity == CardRarity.Common)).WithFlags(CardCreationFlags.NoUpgradeRoll);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(CardFactory.CreateForReward(Owner,1, options).FirstOrDefault()!.Card, PileType.Deck));
        }
        this.SetEventFinished(this.L10NLookup("BALATRO-SPECTRAL_MERCHANT_EVENT.pages.FINISHED.description"));
    }
    
    private async Task Leave()
    {
        this.SetEventFinished(this.L10NLookup("BALATRO-SPECTRAL_MERCHANT_EVENT.pages.LEAVE.description"));
    }

    
    private void SetRandomCards()
    {
        var list = this.Owner!.Deck.Cards.Where(c => c.IsRemovable && c.Rarity != CardRarity.Basic).ToList();
        var randomCards = list.TakeRandom(3, Rng).ToList();
        _randomCardToLoseRare = randomCards[0];
        _randomCardToLoseUncommon = randomCards[1];
        _randomCardToLoseCommon = randomCards[2];
        ((StringVar) this.DynamicVars["RandomCardRare"]).StringValue = this._randomCardToLoseRare.Title;
        ((StringVar) this.DynamicVars["RandomCardUncommon"]).StringValue = this._randomCardToLoseUncommon.Title;
        ((StringVar) this.DynamicVars["RandomCardCommon"]).StringValue = this._randomCardToLoseCommon.Title;
    }
    
    public override string CustomInitialPortraitPath => "res://Balatro/images/events/spectral_merchant.png";

}

