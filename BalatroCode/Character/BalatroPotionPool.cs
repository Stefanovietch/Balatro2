using BaseLib.Abstracts;
using Balatro.BalatroCode.Extensions;
using Balatro.BalatroCode.Potions;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Character;

public class BalatroPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Balatro.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
    
    protected override IEnumerable<PotionModel> GenerateAllPotions()
    {
        return [
            ModelDb.Potion<DejaVu>(),
            ModelDb.Potion<Justice>(),
            ModelDb.Potion<Medium>(),
            ModelDb.Potion<Talisman>(),
            ModelDb.Potion<TheChariot>(),
            ModelDb.Potion<TheLovers>(),
            ModelDb.Potion<TheMagician>(),
            ModelDb.Potion<Trance>()
        ];
    }
}