using System;

namespace AeonCore
{
	public class Hero
	{
		protected StatsContainer Stats { get; }
		public IReadOnlyStats StatsRO => Stats;

		public bool IsAlive => StatsRO.DynamicValue<Health>() > 0;

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

		internal int Wage(int amount) => Money += amount;

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

		internal virtual Damage GetDamageTo(Hero enemy)
		{
			int phys = StatsRO.ConvInt<Attack>();
			int magic = StatsRO.ConvInt<Magic>();
			bool procCrit = Game.RNG.NextDouble() < StatsRO.Converted<CritChance>();
			return new Damage {
				Instigator = this,
				Phys = procCrit ? (int) (phys * StatsRO.Converted<CritDamage>()) : phys,
				Magic = magic,
				IsCrit = procCrit,
			};
		}

		internal virtual Damage ReceiveDamage(Damage damage)
		{
			int phys = (int)(damage.Phys * (1 - StatsRO.Converted<Armor>()) - StatsRO.Converted<Block>());
			var d = new Damage {
				Instigator = damage.Instigator,
				Phys = phys < 0 ? 0 : phys,
				Magic = damage.Magic,
				IsCrit = damage.IsCrit,
			};
			Stats.Get<Health>().Value -= (d.Phys + d.Magic); // hack
			return d;
		}

		internal virtual void AfterHit(Damage enemyHit)
		{
			Stats.Get<Health>().AfterHit(this, enemyHit);
			Stats.Get<Income>().AfterHit(this, enemyHit);
		}
	}
}