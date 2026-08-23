using Balatro.BalatroCode.UI;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Runs;

namespace Balatro.BalatroCode.Patches;

public class CharacterSelectScreenPatches
{
    [HarmonyPatch(typeof(NCharacterSelectScreen), "_Ready")]
    public static class CharacterSelectScreenReadyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(NCharacterSelectScreen __instance)
        {
            try
            {
                DeckPanelUI.Attach(__instance);
                StakePanelUI.Attach(__instance);
            }
            catch (Exception ex)
            {
                MainFile.Logger.Warn($"_Ready postfix error: {ex.Message}");
            }
        }
    }


    [HarmonyPatch(typeof(NCharacterSelectScreen), "SelectCharacter")]
    public static class CharacterSelectScreenSelectPatch
    {
        public static void Postfix(NCharacterSelectButton charSelectButton, CharacterModel characterModel)
        {
            try
            {
                var id = characterModel?.Id?.Entry;
                if (string.IsNullOrEmpty(id)) return;

                var isBalatro = id is "BALATRO-BALATRO";
                DeckPanelUI.UpdateRelic(BalatroConfig.SelectedDeck);
                DeckPanelUI.SetVisibility(isBalatro);
                StakePanelUI.SetVisibility(isBalatro);
            }
            catch (Exception ex)
            {
                MainFile.Logger.Warn($"SelectCharacter postfix error: {ex.Message}");
            }
        }
    }
}