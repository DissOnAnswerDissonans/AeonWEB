﻿using System;
using System.Collections.Generic;

namespace AeonCore
{
	abstract public class Shop
	{
		List<Offer> offers;

		public IReadOnlyList<Offer> Offers => offers;

		protected void AddOffer<T>(int amount, int cost) 
			where T : Stat, new() => 
			offers.Add(new Offer(new T() { Value = amount }, cost, false));

		protected void AddOptOffer<T>(int amount, int cost) 
			where T : Stat, new() =>
			offers.Add(new Offer(new T() { Value = amount }, cost, true));
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
			AddOffer<Multiplier>(2, 13);
			AddOffer<Block>(2, 4);
			AddOffer<Armor>(15, 30);
			AddOffer<Regen>(5, 11);

			AddOptOffer<Health>(220, 87);
			AddOptOffer<Attack>(60, 120);
			AddOptOffer<Magic>(46, 90);
			AddOptOffer<CritChance>(40, 104);
			AddOptOffer<CritDamage>(120, 105);
			AddOptOffer<Multiplier>(20, 120);
			AddOptOffer<Block>(80, 130);
			AddOptOffer<Armor>(66, 120);
			AddOptOffer<Regen>(62, 115);
		}
	}

	public class Offer
	{
		Stat Stat { get; }
		int Cost { get; }
		bool IsOpt { get; }

		internal Offer(Stat stat, int cost, bool opt = false)
		{
			Stat = stat;
			Cost = cost;
			IsOpt = opt;
		}

		public bool TryToBuy(Hero hero)
		{
			return false;
		}
	}
}