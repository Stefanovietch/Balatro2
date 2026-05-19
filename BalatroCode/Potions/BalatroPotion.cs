using BaseLib.Abstracts;
using BaseLib.Utils;
using Balatro.BalatroCode.Character;

namespace Balatro.BalatroCode.Potions;

[Pool(typeof(BalatroPotionPool))]
public abstract class BalatroPotion : CustomPotionModel;