using Balatro.BalatroCode.Extensions;
using Balatro.BalatroCode.Relics;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
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

public class JackInTheBoxEvent() : CustomEventModel()
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new MaxHpVar(30),
        new HpLossVar(6),
        new GoldVar(300)
    ];
    
    public override bool IsAllowed(IRunState runState) => runState.Players.Any(p => p.Character is Character.Balatro);
    
    public override ActModel[] Acts =>
    [
        ModelDb.Act<Glory>()
    ];
    
    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
        [
            Owner!.Gold >= DynamicVars.Gold.BaseValue
                ? Option(EnterBox).ThatDecreasesMaxHp(DynamicVars.MaxHp.IntValue)
                : new EventOption(this, null, "BALATRO-JACK_IN_THE_BOX_EVENT.pages.INITIAL.options.ENTER_BOX_LOCKED"),

            Option(KickBox).ThatDoesDamage(DynamicVars.HpLoss.IntValue),
            Option(Leave)
        ];
    
    public async Task EnterBox()
    {
        if (DynamicVars.MaxHp.BaseValue < Owner!.Creature.MaxHp)
        {
            await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), this.Owner.Creature,
                DynamicVars.MaxHp.BaseValue,
                false);
            await PlayerCmd.LoseGold(DynamicVars.Gold.IntValue, Owner);
            this.SetEventState(this.L10NLookup("BALATRO-JACK_IN_THE_BOX_EVENT.pages.ENTER_BOX.description"), [
                Option(GainPalette, HoverTipFactory.FromRelic<Palette>(), "ENTER_BOX"),
                Option(GainNachoTong, HoverTipFactory.FromRelic<NachoTong>(), "ENTER_BOX"),
                Option(Duplicate, "ENTER_BOX")
            ]);
        }
        else
        {
            await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), Owner.Creature, Owner.Creature.MaxHp - 1, false);
            await CreatureCmd.Kill(Owner.Creature);
        }
        
    }

    public async Task GainPalette()
    {
        await RelicCmd.Obtain(ModelDb.Relic<Palette>().ToMutable(), Owner!);
        this.SetEventFinished(this.L10NLookup("BALATRO-JACK_IN_THE_BOX_EVENT.pages.FINISHED.description"));
    }
    
    public async Task GainNachoTong()
    {
        await RelicCmd.Obtain(ModelDb.Relic<NachoTong>().ToMutable(), Owner!);
        this.SetEventFinished(this.L10NLookup("BALATRO-JACK_IN_THE_BOX_EVENT.pages.FINISHED.description"));
    }
    
    public async Task Duplicate()
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(this.L10NLookup("BALATRO-JACK_IN_THE_BOX_EVENT.pages.DUPLICATE.selectionScreenPrompt"), 1);
        CardModel? mutableCard = (await CardSelectCmd.FromDeckGeneric(Owner!, prefs, c => c.Type != CardType.Quest)).FirstOrDefault();
        if (mutableCard == null)
            return;
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(Owner!.RunState.CloneCard(mutableCard), PileType.Deck));
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(Owner!.RunState.CloneCard(mutableCard), PileType.Deck));
        this.SetEventFinished(this.L10NLookup("BALATRO-JACK_IN_THE_BOX_EVENT.pages.FINISHED.description"));
    }
    
    public async Task KickBox()
    {
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner!.Creature, DynamicVars.HpLoss.IntValue, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        await PlayerCmd.GainGold(69, Owner);
        this.SetEventFinished(this.L10NLookup("BALATRO-JACK_IN_THE_BOX_EVENT.pages.HURT.description"));
    }
    
    private async Task Leave()
    {
        this.SetEventFinished(this.L10NLookup("BALATRO-JACK_IN_THE_BOX_EVENT.pages.LEAVE.description"));
    }
    
    public override string CustomInitialPortraitPath => "res://Balatro/images/events/jack_in_the_box.png";
}

