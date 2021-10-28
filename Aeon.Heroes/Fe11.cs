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
		int _battles = 0;

		public Fe11()
		{
			Stats.Set<Health>(Stats.Get<Health>().Value / 2);
			Stats.Set<Attack>(Stats.Get<Attack>().Value * 2);
		}

		public override void OnBattleStart(IBattler enemy)
		{
			base.OnBattleStart(enemy);
			++_battles;
			Stats.SetDyn<Income>(2 + _battles / 10);
		}
	}
}
