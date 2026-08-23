using Balatro.BalatroCode.Extensions;

namespace Balatro.BalatroCode.Afflictions;

public class Wheeled : BalatroAfflictions
{
    public override bool CanAfflictUnplayableCards => false;

    public override string? CustomOverlayPath => "res://Balatro/images/afflictions/wheeled/wheeled.tscn";

    public override string? CustomLocalizationKey => "BALATRO-WHEELED";
}