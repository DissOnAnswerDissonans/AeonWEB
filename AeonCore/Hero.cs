using System;

namespace AeonCore
{
	public class Hero
	{
		public int CurrentHealth { get; }

		protected StatsContainer Stats { get; }

		internal protected Hero()
		{
			Stats.Register<Health>	   (100); // здоровье
			Stats.Register<Attack>      (15); // атака
			Stats.Register<Magic>        (0); // магия
			Stats.Register<CritChance>   (0); // критический шанс
			Stats.Register<CritDamage> (150); // критический урон
			Stats.Register<Multiplier>   (0); // прирост
			Stats.Register<Block>        (1); // броня
			Stats.Register<Armor>        (0); // защита
			Stats.Register<Regen>        (1); // регенерация
		}
		

	}
}