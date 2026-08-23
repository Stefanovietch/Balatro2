using Balatro.BalatroCode.Extensions;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Balatro.BalatroCode.Afflictions;

public class BalatroAfflictions : AfflictionModel
{
    public virtual string? CustomOverlayPath => null;
    public virtual string? CustomLocalizationKey => null;
}