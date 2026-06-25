using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;

namespace Balatro.BalatroCode.UI;


[GlobalClass]
public partial class NTopBarMaxGold : NCustomTopBarDisplayElement
{
    public override string ScenePath => "res://Balatro/images/charui/top_bar_max_gold.tscn";
    public override float Width => 200f;
    protected override string IconNodePath => "Control";
    protected override string CountLabelNodePath => "CombatGold";

    public override Func<Player, bool> CanUse =>
        player => player.Character == ModelDb.Character<Character.Balatro>();

    protected override int? GetGoldEarned()
    {
        if (Player?.Character is not Character.Balatro balatro) return null;
        if (Player.PlayerCombatState == null) return 0;
        return balatro.CombatGoldEarned.Get(Player.PlayerCombatState);
    }
    protected override int? GetMaxGold()
    {
        if (Player?.Character is not Character.Balatro balatro) return null;
        return balatro.MaxCombatGold.Get(balatro);
    }
}