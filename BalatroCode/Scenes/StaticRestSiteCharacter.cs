using Balatro.BalatroCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Nodes.RestSite;

namespace Balatro.BalatroCode.Scenes;

public partial class StaticRestSiteCharacter : NRestSiteCharacter
{
    private Sprite2D _sprite;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
        string path = $"decks/{BalatroConfig.SelectedDeck}.png".ImagePath();
        var texture = GD.Load<Texture2D>(path);
        if (texture != null)
            _sprite.Texture = texture;
    }

    public void SetImage(Texture2D texture)
    {
        _sprite.Texture = texture;
    }
}