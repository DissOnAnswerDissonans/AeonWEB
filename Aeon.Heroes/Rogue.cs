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
		[Balance] private decimal rogueHit = .09m;
		[Balance] private decimal enemyHit = .11m;
		[Balance] private decimal battleBonus = .02m;

		private StatDef RogueHitPerc { get; set; }
		private StatDef EnemyHitPerc { get; set; }

		protected override void PostActivate()
		{
			RogueHitPerc.Edit.Convert(x => rogueHit / (1 + battleBonus * x));
			EnemyHitPerc.Edit.Convert(x => enemyHit * (1 + battleBonus * x));
		}



		public override Damage GetDamageTo(IBattler enemy) => base.GetDamageTo(enemy)
			.ModMag(a => a + (int) (enemy.StatsRO.GetDynValue(Health) * EnemyHitPerc.Converted));

		public override Damage ReceiveDamage(Damage damage) => base.ReceiveDamage(damage
			.ModMag(a => a + (int) (StatsRO.GetDynValue(Health) * RogueHitPerc.Converted)));

		public override string AbilityText =>
			$"По себе:{RogueHitPerc.Converted * 100:F2}% По врагу:{EnemyHitPerc.Converted * 100:F2}%";

		public override void AfterBattle(IBattler enemy, bool isWon)
		{
			base.AfterBattle(enemy, isWon);
			RogueHitPerc.Add(1);
			EnemyHitPerc.Add(1);
		}
	}
}