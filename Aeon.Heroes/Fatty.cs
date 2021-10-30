﻿using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Герой ранней и средней стадий игры. Его улучшения дают
	/// на 9% больше здоровья. После каждого Боя он получает +2
	/// регенерации.
	/// </summary>
	public class Fatty : Hero
	{
		private class FattyShop : StandardShop
		{
			protected override void AddOffer<T>(int amount, int cost, bool opt)
			{
				if (typeof(T) == typeof(Health))
					base.AddOffer<T>((int) (amount * HEALTH_MULTIPLIER), cost, opt);
				else
					base.AddOffer<T>(amount, cost, opt);
			}
		}

		private const decimal HEALTH_MULTIPLIER = 1.095m;
		private readonly Stat _regenBonus = Stat.Make<Regen>(2);
		private int _addedRegen;
		public override string AbilityText => $"Нажрал +{_addedRegen} регенерации";

		public Fatty() => Shop = new FattyShop();

		public override void AfterBattle(IBattler enemy, bool isWon)
		{
			base.AfterBattle(enemy, isWon);
			Stats.AddStat(_regenBonus);
			_addedRegen += _regenBonus.Value;
		}
	}
}