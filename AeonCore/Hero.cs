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
			Stats.RegisterDyn<Health>  (100); // здоровье
			Stats.Register<Attack>      (15); // атака
			Stats.Register<Magic>        (0); // магия
			Stats.Register<CritChance>   (0); // критический шанс
			Stats.Register<CritDamage> (150); // критический урон
			Stats.RegisterDyn<Income>    (0); // прирост
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
			Stats.OnBattleStart(this, enemy); // TODO: обходить динамически
		}

		internal virtual Damage GetDamageTo(Hero enemy)
		{
			int phys = (int)(StatsRO.ConvInt<Attack>() * StatsRO.DynConverted<Income>());
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

			Stats.Modify<Health>(-(d.Phys + d.Magic));
			return d;
		}

		internal virtual void AfterHit(Damage enemyHit)
		{
			Stats.AfterHit(this, enemyHit);
		}
	}
}