using Balatro.BalatroCode.Character;
using BaseLib.Abstracts;
using BaseLib.Utils;

namespace Balatro.BalatroCode.Potions;

[Pool(typeof(BalatroPotionPool))]
public abstract class BalatroPotion : CustomPotionModel;