using Balatro.BalatroCode.Cards;
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
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Balatro.BalatroCode.Events;

public class WeirdJokerEvent() : CustomEventModel()
{
    
    public override bool IsAllowed(IRunState runState)
    {
        return runState.TotalFloor > 6 && runState.Players.All(p => p.Deck.Cards.Count(c => c.IsRemovable) >= 3) && runState.Players.Any(p => p.Character is Character.Balatro);
    }
    
    public override ActModel[] Acts =>
    [
        ModelDb.Act<Hive>(),
    ];
    
    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
        [
            Option(Wraith),
            Option(Immolate),
            Option(BlackHole, [HoverTipFactory.FromCard<Dilation>()])
        ];

    public async Task Wraith()
    {
        CardCreationOptions options = new CardCreationOptions([Owner!.Character.CardPool], CardCreationSource.Other, CardRarityOddsType.Uniform, (Func<CardModel, bool>) (c => c.Rarity == CardRarity.Rare)).WithFlags(CardCreationFlags.NoUpgradeRoll);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(CardFactory.CreateForReward(Owner,1, options).FirstOrDefault()!.Card, PileType.Deck));
        await PlayerCmd.SetGold(0, Owner);
        this.SetEventFinished(this.L10NLookup("BALATRO-WEIRD_JOKER_EVENT.pages.FINISHED.description"));
    }
    
    public async Task Immolate()
    {
        await PlayerCmd.GainGold(200, Owner!);
        var cards = Owner!.Deck.Cards.Where(c => c.IsRemovable).TakeRandom(5, Rng).ToList();
        await CardPileCmd.RemoveFromDeck(cards);
        this.SetEventFinished(this.L10NLookup("BALATRO-WEIRD_JOKER_EVENT.pages.FINISHED.description"));
    }
    
    public async Task BlackHole()
    {
        foreach (CardModel card in PileType.Deck.GetPile(Owner!).Cards.Where(c => c.IsUpgradable).ToList())
        {
            CardCmd.Upgrade(card, CardPreviewStyle.MessyLayout);
            await Cmd.CustomScaledWait(0.1f, 0.2f);
        }
        await CardPileCmd.AddCurseToDeck<Dilation>(Owner!);
        this.SetEventFinished(this.L10NLookup("BALATRO-WEIRD_JOKER_EVENT.pages.FINISHED.description"));
    }
    
    public override string CustomInitialPortraitPath => "res://Balatro/images/events/weird_joker.png";

}

