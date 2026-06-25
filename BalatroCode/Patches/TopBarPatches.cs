using Balatro.BalatroCode.Relics;
using Balatro.BalatroCode.UI;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Patches;

public class TopBarPatches
{
    [HarmonyPatch(typeof(NTopBar), nameof(NTopBar.Initialize))]
    internal class PatchTopBarInitialize
    {
        [HarmonyPostfix]
        private static void AddRegisteredElements(NTopBar __instance, IRunState runState)
        {
            if (!BalatroConfig.GoldCap) return;
            var localPlayer = LocalContext.GetMe(runState);
            if (localPlayer == null) return;

            var rightContainer = __instance.GetNodeOrNull<HBoxContainer>("RightAlignedStuff");
            if (rightContainer == null) return;

            foreach (var type in TopBarElementRegistry.Types)
            {
                var (scenePath, canUse, width) = TopBarElementRegistry.ReadMetadata(type);
                if (!canUse(localPlayer)) continue;

                var scene = ResourceLoader.Load<PackedScene>(scenePath);
                if (scene == null) continue;

                var node = scene.Instantiate<Control>();
                node.CustomMinimumSize = new Vector2(width, 0);
                node.SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin;

                rightContainer.AddChild(node);
                rightContainer.MoveChild(node, 3);
                if (node is ITopBarElement element) element.Initialize(localPlayer);
            }
        }
    }
}