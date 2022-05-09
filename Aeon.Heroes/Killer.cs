using Aeon.Core;

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
		[Balance] private decimal conversionRate = 0.15m;
		[Balance] private int lvlCoeff = 75;
		[Balance] private int attackBonus;

		protected override void PostActivate() { }
		private int _totalDamage;
		private int _abilityLevel;

		private int NextLevel => lvlCoeff * (_abilityLevel + 1) * (_abilityLevel + 2);

		private void AddToDamage(int damage)
		{
			_totalDamage += damage;
			while (_totalDamage >= NextLevel) {
				_abilityLevel++;
				Stats.AddToValue(Attack, attackBonus);
			}
		}

		public override Damage GetDamageTo(IBattler enemy)
		{
			Damage d = base.GetDamageTo(enemy);
			int converted = (int)(d.Phys * conversionRate);
			return new Damage(d.Instigator, d.Phys - converted, d.Magic + converted, d.IsCrit);
		}

		public override void AfterHit(Damage enemyHit, Damage ourHit)
		{
			base.AfterHit(enemyHit, ourHit);
			AddToDamage(ourHit.Phys + ourHit.Magic);
		}

		public override string AbilityText =>
			$"+{_abilityLevel * attackBonus} АТК, Для +{attackBonus}: {NextLevel - _totalDamage} урона";
	}
}