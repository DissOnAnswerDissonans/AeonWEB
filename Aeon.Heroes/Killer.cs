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
		private StatDef TotalDamage { get; set; }
		private StatDef BonusAttack { get; set; }
		private StatDef ToNextLevel { get; set; }
		private int _lvl = 0;

		private int NextLevel => lvlCoeff * (_lvl + 1) * (_lvl + 2);

		private void AddToDamage(int damage)
		{
			TotalDamage.Add(damage);
			while (TotalDamage >= NextLevel) {
				_lvl++;
				BonusAttack.Add(attackBonus);
				Stats.AddToValue(Attack, attackBonus);
			}
		}

		public override void OnRoundStart() => ToNextLevel.Set(NextLevel - TotalDamage);

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
			$"+{BonusAttack * attackBonus} АТК, Для +{attackBonus}: {ToNextLevel} урона";
	}
}