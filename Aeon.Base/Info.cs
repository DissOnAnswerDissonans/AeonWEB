using System;
using System.Collections.Generic;
using System.Text;

namespace Aeon.Base
{
	public class HeroInfo
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string AssemblyName { get; set; }
		public string NameID => $"{AssemblyName}:{Name}";
	}

	public class HeroSelection
	{
		public string Nickname { get; set; }
		public HeroInfo Hero { get; set; }
	}

	public class StatInfo
	{
		public string ID { get; set; }
		public string StatName { get; set; }
		public string OwnerID { get; set; }
	}

	public class StatData
	{
		public string StatId { get; set; }
		public int RawValue { get; set; }
		public decimal? Value { get; set; }
	}

	public class OfferData
	{
		public StatData StatAmount { get; set; }
		public int Cost { get; set; }
		public bool IsOpt { get; set; }
	}

	public class BalanceSheet
	{
		public Dictionary<string, BalanceValue> GlobalBalance { get; set; }
		public Dictionary<string, Dictionary<string, BalanceValue>> HeroesBalance { get; set; }
		public List<OfferData> StandardOffers { get; set; }
	}

	public class BalanceValue
	{
		public decimal BaseValue { get; set; }
		public string ScalerStatID { get; set; }
		public decimal ScalingValue { get; set; }

		public static implicit operator BalanceValue(decimal d) 
			=> new BalanceValue { BaseValue = d };
	}

	public class TotalInfoDownload
	{
		public List<HeroInfo> HeroesList { get; set; }
		public List<StatInfo> StatsList { get; set; }

		public BalanceSheet Balance { get; set; }
	}
}
