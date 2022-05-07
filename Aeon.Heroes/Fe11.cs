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

		private static decimal StartHealthMult;
		private static decimal StartAttackMult;
		private static int InitIncome;
		private static int BattlesForBonus;

		public Fe11()
		{
			Stats.Set<Health>((int) (Stats.GetStat<Health>().Value * StartHealthMult));
			Stats.Set<Attack>((int) (Stats.GetStat<Attack>().Value * StartAttackMult));
		}

		public override void OnBattleStart(IBattler enemy)
		{
			base.OnBattleStart(enemy);
			++_battles;
			Stats.SetDyn<Income>(InitIncome + _battles / BattlesForBonus);
		}
	}
}