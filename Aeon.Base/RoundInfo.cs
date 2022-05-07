using System.Collections.Generic;

namespace AeonServer.Models
{
	public class RoundInfo
	{
		public int Number { get; set; }
		public List<Battle> Battles { get; set; }
		public int Wage { get; set; }

		public class Battle
		{
			public Contender First { get; set; }
			public Contender Second { get; set; }
			public int Prize { get; set; }
		}

		public class Contender
		{
			public string PlayerName { get; set; }
			public string HeroID { get; set; }
			public int Points { get; set; }
		}
	}
}