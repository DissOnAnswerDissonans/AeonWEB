using System;

namespace Aeon.Core
{

	interface IShopper
	{
		int Money { get; }
		Shop Shop { get; }

		bool TryBuyOffer(Offer offer);
	}


	public class Hero : IBattler, IShopper
	{
		public StatsContainer Stats { get; private set; }
		public IReadOnlyStats StatsRO => Stats;

		public bool IsAlive => StatsRO.DynamicValue<Health>() > 0;

		public int Money { get; private set; }
		public Shop Shop { get; protected set; }

		internal protected Hero()
		{
			ResetStats();
			Shop = new StandardShop();
		}

		protected void ResetStats()
		{
			Stats = new StatsContainer();
			Stats.RegisterDyn<Health>(100); // здоровье
			Stats.Register<Attack>(15); // атака
			Stats.Register<Magic>(0); // магия
			Stats.Register<CritChance>(0); // критический шанс
			Stats.Register<CritDamage>(150); // критический урон
			Stats.RegisterDyn<Income>(0); // прирост
			Stats.Register<Block>(1); // броня
			Stats.Register<Armor>(0); // защита
			Stats.Register<Regen>(1); // регенерация
		}

		public int Wage(int amount) => Money += amount >= 0? amount 
			: throw new ArgumentOutOfRangeException();

		public int Spend(int amount)
		{
			if (Money < amount)
				throw new ArgumentException("Not enough money", nameof(amount));
			return Money -= amount;
		}

		public virtual bool TryBuyOffer(Offer offer)
		{
			if (offer.Cost > Money) return false;
			Stats.AddStat(offer.Stat);
			Spend(offer.Cost);
			return true;
		}

		public virtual void OnBattleStart(IBattler enemy)
		{
			Stats.SetDyn<Health>(StatsRO.ConvInt<Health>());
			Stats.SetDyn<Income>(0);
		}

		public virtual Damage GetDamageTo(IBattler enemy)
		{
			decimal d = StatsRO.DynConverted<Income>();
			int phys = (int)(StatsRO.ConvInt<Attack>() * d);
			int magic = StatsRO.ConvInt<Magic>();
			bool procCrit = Game.RNG.NextDouble() < (double) StatsRO.Converted<CritChance>();
			return new Damage {
				Instigator = this,
				Phys = procCrit ? (int) (phys * StatsRO.Converted<CritDamage>()) : phys,
				Magic = magic,
				IsCrit = procCrit,
			};
		}

		public virtual Damage ReceiveDamage(Damage damage)
		{
			int phys = (int)(damage.Phys * (1 - StatsRO.Converted<Armor>()) - StatsRO.Converted<Block>());
			var d = new Damage {
				Instigator = damage.Instigator,
				Phys = phys < 0 ? 0 : phys,
				Magic = damage.Magic,
				IsCrit = damage.IsCrit,
			};

			Stats.ModifyDyn<Health>(-(d.Phys + d.Magic));
			return d;
		}

		public virtual void AfterHit(Damage enemyHit, Damage ourHit)
		{
			if (enemyHit.Phys > 0)
				Stats.ModifyDyn<Health>(StatsRO.ConvInt<Regen>());
			Stats.ModifyDyn<Income>(1);
		}

		public virtual void AfterBattle(IBattler enemy, bool isWon)
		{

		}

		public virtual string AbilityText => "Нет способности";
		public virtual bool UseAbility() => false;

	}
}