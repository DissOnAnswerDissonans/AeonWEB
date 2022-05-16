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

		public virtual void AddOffer(string id, int amount, int cost, bool opt = false) =>
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

	public class BalancedShop : Shop
	{
		public BalancedShop(BalanceSheet balance)
		{
			foreach(var offer in balance.StandardOffers) {
				AddOffer(offer.StatAmount.StatId, offer.StatAmount.RawValue, offer.Cost, offer.IsOpt);
			}
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