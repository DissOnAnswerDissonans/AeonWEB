using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using StatData = Aeon.Base.StatData;

namespace Aeon.Base
{

	public class PlayerStatus
	{
		public string Name { get; set; }
		public bool IsConnected { get; set; }
	}

	public class Hero
	{
		public string HeroId { get; set; }
		public int Money { get; set; }
		public List<StatData> Stats { get; set; }
	}

	public class FinalResult
	{
		public RoundScoreSummary Scores { get; set; }
		public string Winner { get; set; }
		public Dictionary<string, Hero> Players { get; set; }
	}
}