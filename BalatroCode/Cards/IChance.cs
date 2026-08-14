using Balatro.BalatroCode.Cards;
using Balatro.BalatroCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public interface IChance
{
}

public static class ChanceMethods
{
    public static int GetNumerator(this IChance _, Player? player)
    {
        var power = player?.Creature.GetPower<OopsAll6sPower>();
        if (power == null) return 1;
        return (int)Math.Pow(2, power.Amount);
    }

    public static bool RollChance(this IChance IChance, Player player, int chance)
    {
        chance = chance < 1 ? 1 : chance;
        return (float)IChance.GetNumerator(player) / chance >= player.RunState.Rng.Niche.NextFloat();
    }
}