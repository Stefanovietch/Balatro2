using Balatro.BalatroCode.UI.Decks;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

namespace Balatro.Balatro.images.ui;

public partial class DeckPanel : Control
{
    public static AddedNode<NCharacterSelectScreen, DeckPanel> Node = new((charselect) =>
        {
            if SelectedCharacter
            var control = new DeckPanel();
            control.AddChild(new AbandonedDeck());
            return control;
        }
    );
}