using Balatro.BalatroCode.Extensions;

namespace Balatro.BalatroCode.Afflictions;

public class Oxed : BalatroAfflictions
{
    public override bool CanAfflictUnplayableCards => false;

    public override string? CustomOverlayPath => "res://Balatro/images/afflictions/oxed/oxed.tscn";

    public override string? CustomLocalizationKey => "BALATRO-OXED";
}