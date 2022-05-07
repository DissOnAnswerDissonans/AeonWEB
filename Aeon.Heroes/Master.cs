﻿using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Герой поздней стадии игры. У него есть характеристика —
	/// вампиризм. Изначально она равна 15%. После каждого Боя
	/// вампиризм увеличивается на 0.6%. После каждого удара
	/// Повелитель восстанавливает здоровье на 𝑑𝐻 = 𝑑 × 𝑐𝑉,
	/// где dH — восстанавливаемое здоровье, d — урон, cV —
	/// вампиризм.
	/// </summary>
	public class Master : Hero
	{
		private static decimal vampCoeffStart;
		private static decimal vampAdder;

		private decimal _vamp_coeff = vampCoeffStart;

		private int Vamp => (int) (StatsRO.Converted<Attack>() * _vamp_coeff);

		public override void AfterHit(Damage enemyHit, Damage ourHit)
		{
			base.AfterHit(enemyHit, ourHit);
			Stats.ModifyDyn<Health>(Vamp);
		}

		public override void AfterBattle(IBattler enemy, bool isWon)
		{
			base.AfterBattle(enemy, isWon);
			_vamp_coeff += vampAdder;
		}

		public override string AbilityText => $"Вампиризм {_vamp_coeff:P} ({Vamp})";
	}
}