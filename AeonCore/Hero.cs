using System;

namespace AeonCore
{
	public class Hero
	{
		//public int CurrentHealth { get; private set; }

		protected StatsContainer Stats { get; }
		public IReadOnlyStats StatsRO => Stats;

		public int Money { get; private set; }
		public Shop Shop { get; }

		internal protected Hero()
		{
			Stats.Register<Health>	   (100); // здоровье
			Stats.Register<Attack>      (15); // атака
			Stats.Register<Magic>        (0); // магия
			Stats.Register<CritChance>   (0); // критический шанс
			Stats.Register<CritDamage> (150); // критический урон
			Stats.Register<Income>       (0); // прирост
			Stats.Register<Block>        (1); // броня
			Stats.Register<Armor>        (0); // защита
			Stats.Register<Regen>        (1); // регенерация
		}

		internal int Spend(int amount)
		{
			if (Money < amount)
				throw new ArgumentException("Not enough money", nameof(amount));
			return Money -= amount;
		}

		internal void AddStat(Stat stat) => Stats.AddStat(stat);


		internal virtual void OnBattleStart(Hero enemy)
		{
			Stats.Get<Health>().OnBattleStart(); // TODO: обходить динамически
			Stats.Get<Income>().OnBattleStart();
		}
	}
}