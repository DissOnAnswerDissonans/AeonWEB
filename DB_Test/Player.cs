using System;
using System.Collections.Generic;

namespace DB_Test;

public partial class Player
{
	public Player()
	{
		GamePlayer1s = new HashSet<Game>();
		GamePlayer2s = new HashSet<Game>();
	}

	public int Id { get; set; }
	public string Nickname { get; set; } = null!;
	public int? Pwhash { get; set; }
	public decimal? ValueElo { get; set; }

	public virtual ICollection<Game> GamePlayer1s { get; set; }
	public virtual ICollection<Game> GamePlayer2s { get; set; }
}
