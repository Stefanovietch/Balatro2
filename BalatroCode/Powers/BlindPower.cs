using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
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
        [BlindType.TheArm] = () => new TheArmPower(),
        [BlindType.TheClub] = () => new TheClubPower(),
        [BlindType.TheEye] = () => new TheEyePower(),
        [BlindType.TheFish] = () => new TheFishPower(),
        [BlindType.TheHook] = () => new TheHookPower(),
        [BlindType.TheHouse] = () => new TheHousePower(),
        [BlindType.TheOx] = () => new TheOxPower(),
        [BlindType.ThePsychic] = () => new ThePsychicPower(),
        [BlindType.TheWall] = () => new TheWallPower(),
        [BlindType.TheWheel] = () => new TheWheelPower(),
        [BlindType.TheWater] = () => new TheWaterPower(),
        [BlindType.TheManacle] = () => new TheManaclePower(),
        [BlindType.TheMouth] = () => new TheMouthPower(),
        [BlindType.ThePlant] = () => new ThePlantPower(),
        [BlindType.TheNeedle] = () => new TheNeedlePower(),
        [BlindType.TheHead] = () => new TheHeadPower(),
        [BlindType.TheTooth] = () => new TheToothPower(),
        [BlindType.TheFlint] = () => new TheFlintPower(),
        [BlindType.TheMark] = () => new TheMarkPower(),
        [BlindType.TheGoad] = () => new TheGoadPower(),
        [BlindType.TheSerpent] = () => new TheSerpentPower(),
        [BlindType.TheWindow] = () => new TheWindowPower(),

        [BlindType.AmberAcorn] = () => new AmberAcornPower(),
        [BlindType.VerdantLeaf] = () => new VerdantLeafPower(),
        [BlindType.VioletVessel] = () => new VioletVesselPower(),
        [BlindType.CrimsonHeart] = () => new CrimsonHeartPower(),
        [BlindType.CeruleanBell] = () => new CeruleanBellPower()
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
        if (exclude == null) exclude = [];
        return (isBoss ? BossTypes : Types).Where(exclude.Contains)
            .TakeRandom(1, creature.CombatState.RunState.Rng.MonsterAi).First();
    }

    public static BalatroPower GetRandomBlindPower(Creature creature, bool isBoss = false,
        List<BlindType>? exclude = null)
    {
        var type = GetRandomBlind(creature, isBoss, exclude);
        return CreatePower(type);
    }
}