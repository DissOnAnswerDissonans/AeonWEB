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
		const decimal DMG_BOOST = .39m;

		public override Damage GetDamageTo(IBattler enemy)
		{
			var pBoost = 1 - StatsRO.DynamicValue<Health>() / (decimal) StatsRO.ConvInt<Health>();
			return base.GetDamageTo(enemy).ModPhys(d => (int) (d * (1 + pBoost * DMG_BOOST)));
		}

		public override string AbilityText => "Озверение на 39% от -доли ХП";
	}
}
