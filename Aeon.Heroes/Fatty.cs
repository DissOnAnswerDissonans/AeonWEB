using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Герой ранней и средней стадий игры. Его улучшения дают
	/// на 9% больше здоровья. После каждого Боя он получает +2
	/// регенерации.
	/// </summary>
	public class Fatty : Hero
	{
		class FattyShop : StandardShop
		{
			protected override void AddOffer<T>(int amount, int cost, bool opt)
			{
				if (typeof(T) == typeof(Health))
					base.AddOffer<T>((int)(amount * HEALTH_MULTIPLIER), cost, opt);
				else
					base.AddOffer<T>(amount, cost, opt);
			}
		}

		const decimal HEALTH_MULTIPLIER = 1.095m;
		readonly Stat REGEN_BONUS = Stat.Make<Regen>(2);

		public Fatty()
		{
			Shop = new FattyShop();
		}

		public override void AfterBattle(IBattler enemy)
		{
			base.AfterBattle(enemy);
			Stats.AddStat(REGEN_BONUS);
		}
	}
}
