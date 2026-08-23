using Balatro.BalatroCode.Extensions;

namespace Balatro.BalatroCode.Afflictions;

public class Armed : BalatroAfflictions
{
    public override bool CanAfflictUnplayableCards => false;

    public override string? CustomOverlayPath => "res://Balatro/images/afflictions/armed/armed.tscn";

    public override string? CustomLocalizationKey => "BALATRO-ARMED";
}