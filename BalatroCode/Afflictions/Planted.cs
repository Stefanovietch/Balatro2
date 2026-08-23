using Balatro.BalatroCode.Extensions;

namespace Balatro.BalatroCode.Afflictions;

public class Planted : BalatroAfflictions
{
    public override bool CanAfflictUnplayableCards => false;
    public override string? CustomOverlayPath => "res://Balatro/images/afflictions/planted/planted.tscn";

    public override string? CustomLocalizationKey => "BALATRO-PLANTED";
}