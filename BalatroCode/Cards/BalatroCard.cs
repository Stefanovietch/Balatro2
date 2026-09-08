using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Balatro.BalatroCode.Character;
using Balatro.BalatroCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Balatro.BalatroCode.Cards;

[Pool(typeof(BalatroCardPool))]
public abstract class BalatroCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    public override void AfterCreated()
    {
        if (this is IRandomType randomType && randomType.CurrentType == CardType.None && this.CombatState != null) randomType.SetRandomType(this.CombatState.RunState.Rng.Niche);
        base.AfterCreated();
    }
}

[Pool(typeof(TokenCardPool))]
public abstract class BalatroTokenCard(
    int cost,
    CardType type,
    TargetType target
) : CustomCardModel(cost, type, CardRarity.Token, target)
{
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}

[Pool(typeof(CurseCardPool))]
public abstract class BalatroCurseCard(
    int cost,
    TargetType target
) : CustomCardModel(cost, CardType.Curse, CardRarity.Curse, target)
{
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}