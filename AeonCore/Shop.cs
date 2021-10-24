using System;
using System.Collections.Generic;

namespace Aeon.Core
{
	abstract public class Shop : ICloneable
	{
		List<Offer> offers = new();

		public IReadOnlyList<Offer> Offers => offers;

		public object Clone()
		{
			Shop s = (Shop) this.MemberwiseClone();
			s.offers = new List<Offer>();
			foreach (Offer offer in offers)
				s.offers.Add((Offer) offer.Clone());
			return s;
		}

		protected void AddOffer<T>(int amount, int cost) 
			where T : StatType, new() => 
			offers.Add(new Offer(Stat.Make<T>(amount), cost, false));

		protected void AddOptOffer<T>(int amount, int cost) 
			where T : StatType, new() =>
			offers.Add(new Offer(Stat.Make<T>(amount), cost, true));
	}

	public class StandardShop : Shop
	{
		public StandardShop()
		{
			AddOffer<Health>(22, 10);
			AddOffer<Attack>(3, 7);
			AddOffer<Magic>(7, 15);
			AddOffer<CritChance>(5, 15);
			AddOffer<CritDamage>(50, 50);
			AddOffer<Income>(2, 13);
			AddOffer<Block>(2, 4);
			AddOffer<Armor>(15, 30);
			AddOffer<Regen>(5, 11);

			AddOptOffer<Health>(220, 87);
			AddOptOffer<Attack>(60, 120);
			AddOptOffer<Magic>(46, 90);
			AddOptOffer<CritChance>(40, 104);
			AddOptOffer<CritDamage>(120, 105);
			AddOptOffer<Income>(20, 120);
			AddOptOffer<Block>(80, 130);
			AddOptOffer<Armor>(66, 120);
			AddOptOffer<Regen>(62, 115);
		}
	}

	public struct Offer : ICloneable
	{
		public Stat Stat { get; }
		public int Cost { get; }
		public bool IsOpt { get; }

		internal Offer(Stat stat, int cost, bool opt = false)
		{
			Stat = stat;
			Cost = cost;
			IsOpt = opt;
		}

		public override string ToString()
		{
			return IsOpt ? $"опт {Cost,3}$ => {Stat.Value,-3} {Stat.StatType.DebugNames.AliasRU,-3}"
						 : $"{Cost,2}$ => {Stat.Value,-2} {Stat.StatType.DebugNames.AliasRU,-3}";
		}

		//public bool TryToBuy(Hero hero)
		//{
		//	if (hero.Money < Cost) return false;
		//	hero.Spend(Cost);
		//	hero.AddStat(Stat);
		//	return true;
		//}

		public object Clone()
		{
			return new Offer(Stat, Cost, IsOpt);
		}
	}
}