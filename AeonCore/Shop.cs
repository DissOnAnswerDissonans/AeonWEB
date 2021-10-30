using System;
using System.Collections.Generic;

namespace Aeon.Core
{
	public abstract class Shop : ICloneable
	{
		private List<Offer> _offers = new();

		public IReadOnlyList<Offer> Offers => _offers;

		public object Clone()
		{
			var s = (Shop) MemberwiseClone();
			s._offers = new List<Offer>();
			foreach (Offer offer in _offers)
				s._offers.Add((Offer) offer.Clone());
			return s;
		}

		protected virtual void AddOffer<T>(int amount, int cost, bool opt = false)
			where T : StatType, new() =>
			_offers.Add(new Offer(Stat.Make<T>(amount), cost, opt));

		public void ModifyOffers(Func<Offer, bool> predicate, Func<Offer, Offer> func)
		{
			for (int i = 0; i < _offers.Count; ++i) {
				if (predicate(_offers[i])) {
					_offers[i] = func(_offers[i]);
				}
			}
		}

		internal bool CanBuy(Offer offer, IShopper shopper)
		{
			if (!_offers.Contains(offer))
				throw new ArgumentException("No such offer in hero shop", nameof(offer));
			return offer.Cost <= shopper.Money;
		}
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

			AddOffer<Health>(220, 87, opt: true);
			AddOffer<Attack>(60, 120, opt: true);
			AddOffer<Magic>(46, 90, opt: true);
			AddOffer<CritChance>(40, 104, opt: true);
			AddOffer<CritDamage>(120, 105, opt: true);
			AddOffer<Income>(20, 120, opt: true);
			AddOffer<Block>(80, 130, opt: true);
			AddOffer<Armor>(66, 120, opt: true);
			AddOffer<Regen>(62, 115, opt: true);
		}
	}

	public struct Offer : ICloneable
	{
		public Stat Stat { get; }
		public int Cost { get; }
		public bool IsOpt { get; }

		public Offer(Stat stat, int cost, bool opt = false)
		{
			Stat = stat;
			Cost = cost;
			IsOpt = opt;
		}

		public object Clone() => new Offer(Stat, Cost, IsOpt);

		public override string ToString() => IsOpt
			? $"opt {Cost} => {Stat}" : $"{Cost} => {Stat}";
	}
}