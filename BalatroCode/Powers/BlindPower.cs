using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;

namespace Balatro.BalatroCode.Powers;

public interface IBlindPower
{
    bool IsBoss => this.IsBossBlind();

    BlindType BlindType { get; }
}

public enum BlindType
{
    TheArm,
    TheFish,
    TheHook,
    TheHouse,
    TheOx,
    ThePsychic,
    TheWall,
    TheWheel,
    TheWater,
    TheManacle,
    TheEye,
    TheMouth,
    ThePlant,
    TheNeedle,
    TheHead,
    TheTooth,
    TheFlint,
    TheMark,
    TheClub,
    TheGoad,
    TheSerpent,
    TheWindow,

    AmberAcorn,
    VerdantLeaf,
    VioletVessel,
    CrimsonHeart,
    CeruleanBell
}

public static class BlindMethods
{
    private static readonly List<BlindType> Types =
    [
        BlindType.TheArm,
        BlindType.TheClub,
        BlindType.TheFish,
        BlindType.TheHook,
        BlindType.TheHouse,
        BlindType.TheOx,
        BlindType.ThePsychic,
        BlindType.TheWall,
        BlindType.TheWheel,
        BlindType.TheWater,
        BlindType.TheManacle,
        BlindType.TheEye,
        BlindType.TheMouth,
        BlindType.ThePlant,
        BlindType.TheNeedle,
        BlindType.TheHead,
        BlindType.TheTooth,
        BlindType.TheFlint,
        BlindType.TheMark,
        BlindType.TheGoad,
        BlindType.TheSerpent,
        BlindType.TheWindow
    ];

    private static readonly List<BlindType> BossTypes =
    [
        BlindType.AmberAcorn,
        BlindType.VerdantLeaf,
        BlindType.VioletVessel,
        BlindType.CrimsonHeart,
        BlindType.CeruleanBell
    ];

    private static readonly Dictionary<BlindType, Func<BalatroPower>> Factory = new()
    {
        [BlindType.TheArm] = ModelDb.Power<TheArmPower>,
        [BlindType.TheClub] = ModelDb.Power<TheClubPower>,
        [BlindType.TheEye] = ModelDb.Power<TheArmPower>,
        [BlindType.TheFish] = ModelDb.Power<TheEyePower>,
        [BlindType.TheHook] = ModelDb.Power<TheHookPower>,
        [BlindType.TheHouse] = ModelDb.Power<TheHousePower>,
        [BlindType.TheOx] = ModelDb.Power<TheOxPower>,
        [BlindType.ThePsychic] = ModelDb.Power<ThePsychicPower>,
        [BlindType.TheWall] = ModelDb.Power<TheWallPower>,
        [BlindType.TheWheel] = ModelDb.Power<TheWheelPower>,
        [BlindType.TheWater] = ModelDb.Power<TheWaterPower>,
        [BlindType.TheManacle] = ModelDb.Power<TheManaclePower>,
        [BlindType.TheMouth] = ModelDb.Power<TheMouthPower>,
        [BlindType.ThePlant] = ModelDb.Power<ThePlantPower>,
        [BlindType.TheNeedle] = ModelDb.Power<TheNeedlePower>,
        [BlindType.TheHead] = ModelDb.Power<TheHeadPower>,
        [BlindType.TheTooth] = ModelDb.Power<TheToothPower>,
        [BlindType.TheFlint] = ModelDb.Power<TheFlintPower>,
        [BlindType.TheMark] = ModelDb.Power<TheMarkPower>,
        [BlindType.TheGoad] = ModelDb.Power<TheGoadPower>,
        [BlindType.TheSerpent] = ModelDb.Power<TheSerpentPower>,
        [BlindType.TheWindow] = ModelDb.Power<TheWindowPower>,

        [BlindType.AmberAcorn] = ModelDb.Power<AmberAcornPower>,
        [BlindType.VerdantLeaf] = ModelDb.Power<VerdantLeafPower>,
        [BlindType.VioletVessel] = ModelDb.Power<VioletVesselPower>,
        [BlindType.CrimsonHeart] = ModelDb.Power<CrimsonHeartPower>,
        [BlindType.CeruleanBell] = ModelDb.Power<CeruleanBellPower>,
    };

    private static BalatroPower CreatePower(BlindType type)
    {
        return Factory[type]();
    }

    public static bool IsBossBlind(this IBlindPower blindPower)
    {
        return BossTypes.Contains(blindPower.BlindType);
    }

    private static BlindType GetRandomBlind(Creature creature, bool isBoss = false, List<BlindType>? exclude = null)
    {
        if (creature.CombatState == null) return BlindType.TheArm;
        exclude ??= [];
        var possibleBlinds = (isBoss ? BossTypes : Types).ToList();
        foreach (var type in exclude) possibleBlinds.Remove(type);
        return creature.CombatState.RunState.Rng.MonsterAi.NextItem(possibleBlinds);
    }

    public static BalatroPower GetRandomBlindPower(Creature creature, bool isBoss = false,
        List<BlindType>? exclude = null)
    {
        var type = GetRandomBlind(creature, isBoss, exclude);
        return CreatePower(type);
    }
}