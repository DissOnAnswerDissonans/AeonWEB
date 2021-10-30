﻿using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Герой поздней стадии игры. 15% его урона напрямую
	/// вычитаются из текущего здоровья Героя оппонента (не
	/// подвержены уменьшению разными способностями и
	/// характеристиками). Также у Убийцы есть свой счетчик
	/// нанесенного урона.Когда этот счетчик превышает новое
	/// целевое значение — Убийца получает +10 к атаке. Целевые
	/// значения считаются по формуле 𝐾𝑖 = 75 × 𝑖 × (𝑖 + 1), где
	/// Ki — i-тое целевое значение, i — номер целевого значения.
	/// </summary>
	public class Killer : Hero
	{
		private const decimal CONV_RATE = 0.15m;
		private const int LVL_COEFF = 75;

		private readonly Stat _bonus = Stat.Make<Attack>(10);
		private int _totalDamage;
		private int _abilityLevel;

		private int NextLevel => LVL_COEFF * (_abilityLevel + 1) * (_abilityLevel + 2);

		private void AddToDamage(int damage)
		{
			_totalDamage += damage;
			while (_totalDamage >= NextLevel) {
				_abilityLevel++;
				Stats.AddStat(_bonus);
			}
		}

		public override Damage GetDamageTo(IBattler enemy)
		{
			Damage d = base.GetDamageTo(enemy);
			int converted = (int)(d.Phys * CONV_RATE);
			return new Damage(d.Instigator, d.Phys - converted, d.Magic + converted, d.IsCrit);
		}

		public override void AfterHit(Damage enemyHit, Damage ourHit)
		{
			base.AfterHit(enemyHit, ourHit);
			AddToDamage(ourHit.Phys + ourHit.Magic);
		}

		public override string AbilityText =>
			$"+{_abilityLevel * _bonus.Value} АТК, Для +{_bonus.Value}: {NextLevel - _totalDamage} урона";
	}
}