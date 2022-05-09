namespace Aeon.Core
{

	public interface IShopper
	{
		int Money { get; }
		Shop Shop { get; }

		bool TryBuyOffer(Offer offer);
	}

	abstract public class Hero : IBattler, IShopper
	{
		public const string Health = "HP";
		public const string Attack = "ATT";
		public const string Magic = "MAG";
		public const string CritChance = "CHA";
		public const string CritDamage = "DMG";
		public const string Income = "INC";
		public const string Block = "BLK";
		public const string Armor = "ARM";
		public const string Regen = "REG";

		public StatsContainer Stats { get; } = new();
		public IStatContext StatsRO => Stats;

		public bool IsAlive => StatsRO.TryGetDynValue("HP") > 0;

		public int Money { get; private set; }
		public Shop Shop { get; protected set; }

		protected internal void Activate()
		{
			SetStats();
			Shop = new StandardShop();
			PostActivate();
		}

		protected abstract void PostActivate();

		protected void SetStats()
		{
			const decimal COEFF = .0075m;
			Stats.NewStat(Health).Default(100).AddDynamicDefault();
			Stats.NewStat(Attack).Default(15);
			Stats.NewStat(Magic).Default(0);
			Stats.NewStat(CritChance).Default(0).Limit(100).Convert((x, ctx) => x / 100.0m);
			Stats.NewStat(CritDamage).Default(150).Convert((x, ctx) => x / 100.0m);
			Stats.NewStat(Income).Default(0).Convert((x, ctx) => 1 + x / 100.0m).AddDynamic();
			Stats.NewStat(Block).Default(1);
			Stats.NewStat(Armor).Default(0).Convert((x, ctx) => COEFF * x / (1 + COEFF * (decimal) Math.Exp(0.9 * Math.Log(x))));
			Stats.NewStat(Regen).Default(1);
		}

		public int Wage(int amount) => Money += amount >= 0 ? amount
			: throw new ArgumentOutOfRangeException(nameof(amount), "Can't be negative");

		public int Spend(int amount)
		{
			if (Money < amount)
				throw new ArgumentException("Not enough money", nameof(amount));
			return Money -= amount;
		}

		public virtual bool TryBuyOffer(Offer offer)
		{
			if (!Shop.CanBuy(offer, this)) return false;
			Stats.AddToValue(offer.StatID, offer.Value);
			Spend(offer.Cost);
			return true;
		}

		public virtual void OnBattleStart(IBattler enemy) => Stats.ResetDynamic();

		public virtual Damage GetDamageTo(IBattler enemy)
		{
			decimal d = StatsRO.GetDynValue(Income);
			int phys = (int) (StatsRO.ConvertAsIs(Attack) * d);
			int magic = StatsRO.Convert(Magic);
			bool procCrit = Game.RNG.NextDouble() < (double) StatsRO.ConvertAsIs(CritChance);
			return new Damage {
				Instigator = this,
				Phys = procCrit ? (int) (phys * StatsRO.ConvertAsIs(CritDamage)) : phys,
				Magic = magic,
				IsCrit = procCrit,
			};
		}

		public virtual Damage ReceiveDamage(Damage damage)
		{
			int phys = (int)(damage.Phys * (1 - StatsRO.ConvertAsIs(Armor) - StatsRO.ConvertAsIs(Block)));
			var d = new Damage {
				Instigator = damage.Instigator,
				Phys = phys < 0 ? 0 : phys,
				Magic = damage.Magic,
				IsCrit = damage.IsCrit,
			};

			Stats.AddToDynValue(Health, -(d.Phys + d.Magic));
			return d;
		}

		public virtual void AfterHit(Damage enemyHit, Damage ourHit)
		{
			if (enemyHit.Phys > 0)
				Stats.AddToDynValue(Health, StatsRO.Convert(Regen));
			Stats.AddToDynValue(Income, 1);
		}

		public virtual void AfterBattle(IBattler enemy, bool isWon)
		{
		}

		public virtual string AbilityText => "Нет способности";

		public virtual bool UseAbility() => false;
	}
}