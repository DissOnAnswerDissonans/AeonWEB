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
		private const decimal DMG_BOOST = .039m;

		private int Stacks =>
			(int) (StatsRO.DynamicValue<Health>() / (decimal) StatsRO.ConvInt<Health>() * 10);

		public override Damage GetDamageTo(IBattler enemy) =>
			base.GetDamageTo(enemy).ModPhys(d => (int) (d * (1 + Stacks * DMG_BOOST)));

		public override string AbilityText =>
			$"+{DMG_BOOST:P1} АТК за каждые -10% ХП";
	}
}
