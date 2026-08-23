using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public interface IRandomType
{
    CardType CurrentType { get; set; }

    string AllTypesString => $"({nameof(CardType.Skill)}|{nameof(CardType.Attack)}|{nameof(CardType.Power)})";
    List<CardType> AllTypes => [CardType.Attack, CardType.Skill, CardType.Power];
    
    public void SetRandomType()
    {
        CurrentType = AllTypes[Random.Shared.Next(3)];
    }

    public string GetTypeString()
    {
        if (CurrentType == CardType.None) return AllTypesString;
        return CurrentType.ToString();
    }
}