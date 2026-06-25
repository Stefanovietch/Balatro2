using Balatro.BalatroCode.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Balatro.BalatroCode.Cards;

public interface IRandomType
{
   CardType CurrentType
   {
      get;
      set;
   }
   
   public void SetRandomType()
   {
      List<CardType> types = [CardType.Attack, CardType.Skill, CardType.Power];
      CurrentType = types[Random.Shared.Next(types.Count)];
   }

   public string GetTypeString()
   {
      if (CurrentType == CardType.None) return "[Skill/Attack/Power]";
      return CurrentType.ToString();
   }
}
