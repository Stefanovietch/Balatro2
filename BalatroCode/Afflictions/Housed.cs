using Balatro.BalatroCode.Extensions;
using Balatro.BalatroCode.UI;

namespace Balatro.BalatroCode.Afflictions;

public class Housed : BalatroAfflictions
{
    public override bool CanAfflictUnplayableCards => false;

    public override string? CustomOverlayPath => "res://Balatro/images/afflictions/housed/housed.tscn";

    public override string? CustomLocalizationKey => "BALATRO-HOUSED";
}