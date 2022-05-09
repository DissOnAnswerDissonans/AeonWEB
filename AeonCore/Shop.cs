using Aeon.Base;
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
				s._offers.Add(offer);
			return s;
		}

		protected virtual void AddOffer(string id, int amount, int cost, bool opt = false) =>
			_offers.Add(new Offer(id, amount, cost, opt));

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
			AddOffer(Hero.Health, 22, 10);
			AddOffer(Hero.Attack, 3, 7);
			AddOffer(Hero.Magic, 7, 15);
			AddOffer(Hero.CritChance, 5, 15);
			AddOffer(Hero.CritDamage, 50, 50);
			AddOffer(Hero.Income, 2, 13);
			AddOffer(Hero.Block, 2, 4);
			AddOffer(Hero.Armor, 15, 30);
			AddOffer(Hero.Regen, 5, 11);

			AddOffer(Hero.Health, 220, 87, opt: true);
			AddOffer(Hero.Attack, 60, 120, opt: true);
			AddOffer(Hero.Magic, 46, 90, opt: true);
			AddOffer(Hero.CritChance, 40, 104, opt: true);
			AddOffer(Hero.CritDamage, 120, 105, opt: true);
			AddOffer(Hero.Income, 20, 120, opt: true);
			AddOffer(Hero.Block, 80, 130, opt: true);
			AddOffer(Hero.Armor, 66, 120, opt: true);
			AddOffer(Hero.Regen, 62, 115, opt: true);
		}
	}

	public record class Offer
	{
		public string StatID { get; init; }
		public StatValue Value { get; init; }
		public int Cost { get; init; }
		public bool IsOpt { get; init; }

		public Offer(string id, StatValue value, int cost, bool opt = false)
		{
			StatID = id;
			Value = value;
			Cost = cost;
			IsOpt = opt;
		}

		public override string ToString() => IsOpt
			? $"{Value} {StatID} for {Cost} (opt)" : $"{Value} {StatID} for {Cost}";
	}
}