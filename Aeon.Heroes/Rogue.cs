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
		private const decimal ROGUE_HIT = .09m;
		private const decimal ENEMY_HIT = .11m;
		private decimal BattleBonus => 1 + .02m * _battle; 
			// HACK сделал на *, не на ^, отличий при малом кефе не очень много, СПРОСИТЬ!
		private decimal RogueHitPerc => ROGUE_HIT / BattleBonus;
		private decimal EnemyHitPerc => ENEMY_HIT * BattleBonus;

		public override Damage GetDamageTo(IBattler enemy) => base.GetDamageTo(enemy)
			.ModMag(a => a + (int) (enemy.StatsRO.DynConverted<Health>() * EnemyHitPerc));

		public override Damage ReceiveDamage(Damage damage) => base.ReceiveDamage(damage
			.ModMag(a => a + (int) (StatsRO.DynConverted<Health>() * RogueHitPerc)));

		public override string AbilityText =>
			$"По себе:{RogueHitPerc*100:F2}% По врагу:{EnemyHitPerc*100:F2}%";

		public override void AfterBattle(IBattler enemy, bool isWon)
		{
			base.AfterBattle(enemy, isWon);
			++_battle;
		}
	}
}