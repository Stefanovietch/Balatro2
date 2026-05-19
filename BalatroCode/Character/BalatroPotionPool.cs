using BaseLib.Abstracts;
using Balatro.BalatroCode.Extensions;
using Godot;

namespace Balatro.BalatroCode.Character;

public class BalatroPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Balatro.Color;
    

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}