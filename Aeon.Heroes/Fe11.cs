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

		protected override void PostActivate()
		{
			Stats.EditStat(Health).Default((Stats.GetValue(Health) * startHealthMult).TRound());
			Stats.EditStat(Attack).Default((Stats.GetValue(Attack) * startAttackMult).TRound());
		}

		public override void OnBattleStart(IBattler enemy)
		{
			base.OnBattleStart(enemy);
			++_battles;
			Stats.SetDynValue(Income, initIncome + _battles / battlesForBonus);
		}
	}
}