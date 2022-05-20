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

		StatDef StartIncome { get; set; }

		protected override void PostActivate()
		{
			StartIncome.Edit.Convert((x, ctx) => (1 + ctx.ConvertAsIs(Income)).Power(x) - 1).Default(initIncome);
			Stats.EditStat(Health).Default((Stats.GetValue(Health) * startHealthMult).TRound());
			Stats.EditStat(Attack).Default((Stats.GetValue(Attack) * startAttackMult).TRound());
		}

		public override void OnBattleStart(IBattler enemy)
		{
			base.OnBattleStart(enemy);
			++_battles;
			Stats.SetDynValue(Income, initIncome + _battles / battlesForBonus);
		}

		public override void AfterBattle(IBattler enemy, bool isWon) 
		{
			base.AfterBattle(enemy, isWon);
			StartIncome.Set(initIncome + _battles / battlesForBonus);
		}
	}
}