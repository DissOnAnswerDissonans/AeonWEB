using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Универсальный герой.
	/// Перед каждым ударом оба героя теряют часть здоровья.
	/// Эта часть зависит от номера текущего Боя.
	/// Разбойник теряет 0.09ℎ𝑝 / (1 + 0.02*(𝑖−1)) текущего здоровья,
	/// где hp — текущее здоровье Разбойника, i — номер Боя.
	/// Герой противника теряет 0.11𝐸ℎ𝑝 × (1 + 0.02*(𝑖−1)) текущего здоровья,
	/// где Ehp — текущее здоровье противника, i — номер Боя.
	/// </summary>
	public class Rogue : Hero
	{
		private int _battle;
		private static decimal rogueHit = .09m;
		private static decimal enemyHit = .11m;
		private static decimal battleBonus;
		private decimal BattleBonus => 1 + battleBonus * _battle;

		private decimal RogueHitPerc => rogueHit / BattleBonus;

		private decimal EnemyHitPerc => enemyHit * BattleBonus;

		public override Damage GetDamageTo(IBattler enemy) => base.GetDamageTo(enemy)
			.ModMag(a => a + (int) (enemy.StatsRO.DynConverted<Health>() * EnemyHitPerc));

		public override Damage ReceiveDamage(Damage damage) => base.ReceiveDamage(damage
			.ModMag(a => a + (int) (StatsRO.DynConverted<Health>() * RogueHitPerc)));

		public override string AbilityText =>
			$"По себе:{RogueHitPerc * 100:F2}% По врагу:{EnemyHitPerc * 100:F2}%";

		public override void AfterBattle(IBattler enemy, bool isWon)
		{
			base.AfterBattle(enemy, isWon);
			++_battle;
		}
	}
}