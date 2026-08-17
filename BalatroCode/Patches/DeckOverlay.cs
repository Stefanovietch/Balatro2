using BaseLib.Utils;
using Godot;
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
            Texture = GD.Load<Texture2D>("res://Balatro/images/decks/" + BalatroConfig.SelectedDeck + ".png"),
            Modulate = new Color(1f, 1f, 1f, 0.05f),
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Size = frame.Size,
            Position = frame.Position,
            PivotOffset = frame.PivotOffset,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize
        };
        
        cardContainer.AddChild(texRect);
        cardContainer.MoveChild(texRect, cardContainer.GetNode("Frame").GetIndex() + 1);

        return texRect;
    });
}