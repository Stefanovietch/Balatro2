using Balatro.BalatroCode.Cards;
using BaseLib.Abstracts;
using Balatro.BalatroCode.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Character;

public class BalatroCardPool : CustomCardPoolModel
{
    public override string Title => Balatro.CharacterId; //This is not a display name.

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    /* These HSV values will determine the color of your card back.
    They are applied as a shader onto an already colored image,
    so it may take some experimentation to find a color you like.
    Generally they should be values between 0 and 1. */
    public override float H => 0f; //Hue; changes the color.
    public override float S => 0f; //Saturation
    public override float V => 1f; //Brightness

    //Color of small card icons
    public override Color DeckEntryCardColor => new("f0f0f0");
    public override bool IsColorless => false;
    public override IEnumerable<CardModel> AllCards
    {
        get
        {
            var isExtinct = Balatro.GrosMichelExtinct.Get(base.AllCards.First().Owner);
            var cards = base.AllCards;
            return base.AllCards.Where(card => isExtinct
                        ? card is not GrosMichel 
                        : card is not Cavendish);
        }
    }



}