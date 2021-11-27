using System;
using System.Collections.Generic;

namespace DB_Test;

public partial class Hero
{
	public Hero()
	{
		GameHero1Navigations = new HashSet<Game>();
		GameHero2Navigations = new HashSet<Game>();
	}

	public short Id { get; set; }
	public string AsmName { get; set; } = null!;

	public virtual ICollection<Game> GameHero1Navigations { get; set; }
	public virtual ICollection<Game> GameHero2Navigations { get; set; }
}
