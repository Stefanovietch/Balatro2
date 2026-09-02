using Balatro.BalatroCode.Cards;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Balatro.BalatroCode.Patches;

public class DeckOverlay
{
    public static readonly AddedNode<NCard, TextureRect> Node = new((card) =>
    {
        var cardContainer = card.GetChild(0)!;
        var frame = cardContainer.GetNode<Control>("Frame");
        
        var texRect = new TextureRect
        {
            Modulate = new Color(1f, 1f, 1f, 0.07f),
            Visible = false,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Size = frame.Size,
            Position = frame.Position,
            PivotOffset = frame.PivotOffset,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize
        };
        
        cardContainer.AddChild(texRect);
        cardContainer.MoveChild(texRect, frame.GetIndex() + 1);

        return texRect;
    });
    
    [HarmonyPatch(typeof(NCard), "Reload")]
    public static class DeckOverlayReloadPatch
    {
        static void Postfix(NCard __instance)
        {
            var texRect = Node.Get(__instance);

            bool show = __instance.Model is BalatroCard && BalatroConfig.CardOverlay;
            texRect.Visible = show;

            if (show)
            {
                texRect.Texture = GD.Load<Texture2D>("res://Balatro/images/decks/" + BalatroConfig.SelectedDeck + ".png");
            }
        }
    }
}