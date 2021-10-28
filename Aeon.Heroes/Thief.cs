﻿using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Герой ранней и средней стадий игры. После каждого удара
	/// максимальное здоровье Вора увеличивается, а здоровье
	/// противника уменьшается на 1+w, где w — количество побед Вора.
	/// Сука, он мне всю инкапсуляцию поломал, негодяй!
	/// </summary>
	public class Thief : Hero
	{
		int _stealAmount = 1;
		int _stolen = 0;

		public override Damage GetDamageTo(IBattler enemy)
		{
			(enemy as Hero)?.Stats.Modify<Health>(-_stealAmount);
			Stats.Modify<Health>(+_stealAmount);
			_stolen += _stealAmount;
			return base.GetDamageTo(enemy);
		}

		public override void AfterBattle(IBattler enemy, bool isWon)
		{
			base.AfterBattle(enemy, isWon);
			if (isWon) _stealAmount += 1;
		}

		public override string AbilityText => $"Спижжено {_stolen} здоровья";
	}
}