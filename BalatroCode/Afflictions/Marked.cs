using Balatro.BalatroCode.Extensions;

namespace Balatro.BalatroCode.Afflictions;

public class Marked : BalatroAfflictions
{
    public override bool CanAfflictUnplayableCards => false;

    public override string? CustomOverlayPath => "res://Balatro/images/afflictions/marked/marked.tscn";

    public override string? CustomLocalizationKey => "BALATRO-MARKED";
}