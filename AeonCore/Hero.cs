namespace Aeon.Core;

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

	public string ID { get; }
	public StatsContainer Stats { get; private set; } = new();
	public IStatContext StatsRO => Stats;

	public bool IsAlive => StatsRO.TryGetDynValue("HP") > 0;

	public int Money { get; private set; }
	public Shop Shop { get; protected set; }


	public Hero() => ID = GetNameID(GetType());
	public static string GetNameID(Type type) => $"{type.Assembly.GetName().Name}:{type.Name}";
	public Hero Activate(Shop shop = null, StatsContainer stats = null)
	{
		Stats = stats ?? Defaults.Stats;
		Shop = shop ?? Defaults.Shop;
		Money = 100;
		PostActivate();
		Stats.ResetAll();
		return this;
	}

	protected abstract void PostActivate();

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
		decimal incomeX = StatsRO.DynConvertAsIs(Income);
		int phys = (StatsRO.ConvertAsIs(Attack) * incomeX).TRound();
		int magic = StatsRO.Convert(Magic);
		bool procCrit = RNG.TestChance(StatsRO.ConvertAsIs(CritChance));
		return new Damage {
			Instigator = this,
			Phys = procCrit ? (int) (phys * StatsRO.ConvertAsIs(CritDamage)) : phys,
			Magic = magic,
			IsCrit = procCrit,
		};
	}

	public virtual Damage ReceiveDamage(Damage damage)
	{
		int phys = (int)(damage.Phys * (1 - StatsRO.ConvertAsIs(Armor)) - StatsRO.ConvertAsIs(Block));
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

	public int ExpectedDamage => 
		(StatsRO.ConvertAsIs(Attack) * StatsRO.DynConvertAsIs(Income)).TRound() + StatsRO.Convert(Magic);
	public int ExpectedCrit => 
		(StatsRO.ConvertAsIs(Attack) * StatsRO.DynConvertAsIs(Income) * StatsRO.ConvertAsIs(CritDamage)).TRound() + StatsRO.Convert(Magic);

	public virtual bool UseAbility() => false;
}