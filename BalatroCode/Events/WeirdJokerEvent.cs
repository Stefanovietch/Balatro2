using Balatro.BalatroCode.Cards;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Events;

public class WeirdJokerEvent : CustomEventModel
{
    public override ActModel[] Acts =>
    [
        ModelDb.Act<Hive>()
    ];

    public override string CustomInitialPortraitPath => "res://Balatro/images/events/weird_joker.png";

    public override bool IsAllowed(IRunState runState)
    {
        return runState.TotalFloor > 6 && runState.Players.All(p => p.Deck.Cards.Count(c => c.IsRemovable) >= 3) &&
               runState.Players.Any(p => p.Character is Character.Balatro);
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            Option(Wraith),
            Option(Immolate),
            Option(BlackHole, [HoverTipFactory.FromCard<Dilation>()])
        ];
    }

    public async Task Wraith()
    {
        var options =
            new CardCreationOptions([Owner!.Character.CardPool], CardCreationSource.Other, CardRarityOddsType.Uniform,
                (Func<CardModel, bool>)(c => c.Rarity == CardRarity.Rare)).WithFlags(CardCreationFlags.NoUpgradeRoll);
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.Add(CardFactory.CreateForReward(Owner, 1, options).FirstOrDefault()!.Card,
                PileType.Deck));
        await PlayerCmd.SetGold(0, Owner);
        SetEventFinished(L10NLookup("BALATRO-WEIRD_JOKER_EVENT.pages.FINISHED.description"));
    }

    public async Task Immolate()
    {
        await PlayerCmd.GainGold(200, Owner!);
        var cards = Owner!.Deck.Cards.Where(c => c.IsRemovable).TakeRandom(5, Rng).ToList();
        await CardPileCmd.RemoveFromDeck(cards);
        SetEventFinished(L10NLookup("BALATRO-WEIRD_JOKER_EVENT.pages.FINISHED.description"));
    }

    public async Task BlackHole()
    {
        foreach (var card in PileType.Deck.GetPile(Owner!).Cards.Where(c => c.IsUpgradable).ToList())
        {
            CardCmd.Upgrade(card, CardPreviewStyle.MessyLayout);
            await Cmd.CustomScaledWait(0.1f, 0.2f);
        }

        await CardPileCmd.AddCurseToDeck<Dilation>(Owner!);
        SetEventFinished(L10NLookup("BALATRO-WEIRD_JOKER_EVENT.pages.FINISHED.description"));
    }
}