using BaseLib.Abstracts;
using Balatro.BalatroCode.Extensions;
using Balatro.BalatroCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Unlocks;

namespace Balatro.BalatroCode.Character;

public class BalatroRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Balatro.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    protected override IEnumerable<RelicModel> GenerateAllRelics()
    {
        return [
            ModelDb.Relic<Antimatter>(),
            ModelDb.Relic<GlowUp>(),
            ModelDb.Relic<Illusion>(),
            ModelDb.Relic<Liquidation>(),
            ModelDb.Relic<MoneyTree>(),
            ModelDb.Relic<Observatory>(),
            ModelDb.Relic<OverstockPlus>(),
            ModelDb.Relic<Petroglyph>(),
            ModelDb.Relic<PlanetTycoon>(),
            ModelDb.Relic<Recyclomancy>(),
            ModelDb.Relic<RerollGlut>(),
            ModelDb.Relic<Retcon>(),
            ModelDb.Relic<TarotTycoon>(),
        ];
    }
    
    public override IEnumerable<RelicModel> GetUnlockedRelics(UnlockState unlockState)
    {
        List<RelicModel> list = this.AllRelics.Where(relic => relic is not StrikeDummy).ToList();
        return list;
    }
}