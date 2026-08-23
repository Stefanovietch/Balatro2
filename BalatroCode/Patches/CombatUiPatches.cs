using Balatro.BalatroCode.Relics;
using Balatro.BalatroCode.UI;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Patches;

public class CombatUiPatches
{
    
    [HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi._Ready))]
    public static class CombatRetconRerollPatch
    {
        [HarmonyPostfix]
        public static void Postfix(NCombatUi __instance)
        {
            var endTurnButton = __instance.GetNode<Control>("%EndTurnButton");
            
            
            var control = new Control();
            endTurnButton.AddChild(control);
            control.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.CenterTop);
            control.OffsetTop = -100;
            control.OffsetBottom = -27;
            control.MouseFilter = Control.MouseFilterEnum.Ignore;
            
            var button = new NRerollRetconButton();
            button.CustomMinimumSize = new Vector2(276, 73);
            button.Size = button.CustomMinimumSize;
            button.AnchorLeft = 0.5f;
            button.AnchorRight = 0.5f;
            button.OffsetLeft = -276;  
            button.OffsetRight = 276;
            
            control.AddChild(button);
            button.Initialize(__instance);
            
        }
    }
}