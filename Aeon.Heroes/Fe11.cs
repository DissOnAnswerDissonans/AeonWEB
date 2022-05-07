using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Герой поздней стадии игры. Начальное здоровье уменьшено
	/// вдвое, а начальный урон вдвое увеличен. В начале каждого
	/// Боя его текущий прирост срабатывает
	/// 2 раза + 1 раз за каждые 10 боев.
	/// </summary>
	public class Fe11 : Hero
	{
		private int _battles;

		[Balance] private decimal startHealthMult;
		[Balance] private decimal startAttackMult;
		[Balance] private int initIncome;
		[Balance] private int battlesForBonus;

		public Fe11()
		{
			Stats.Set<Health>((int) (Stats.GetStat<Health>().Value * startHealthMult));
			Stats.Set<Attack>((int) (Stats.GetStat<Attack>().Value * startAttackMult));
		}

		public override void OnBattleStart(IBattler enemy)
		{
			base.OnBattleStart(enemy);
			++_battles;
			Stats.SetDyn<Income>(initIncome + _battles / battlesForBonus);
		}
	}
}