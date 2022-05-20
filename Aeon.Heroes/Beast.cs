using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Герой ранней и средней стадии игры. Урон Зверя
	/// увеличивается на 3.9% за каждые недостающие 10%
	/// здоровья. (текущего от максимального)
	/// </summary>
	public class Beast : Hero
	{
		[Balance] private decimal dmgBoost = .039m;


		private int Stacks =>
			(int) (StatsRO.GetDynValue(Health) / (decimal) StatsRO.Convert(Health) * 10);

		public override Damage GetDamageTo(IBattler enemy) =>
			base.GetDamageTo(enemy).ModPhys(d => (int) (d * (1 + Stacks * dmgBoost)));

		public override string AbilityText =>
			$"+{dmgBoost:P1} АТК за каждые -10% ХП";
	}
}