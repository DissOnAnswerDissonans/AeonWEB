using System;
using System.Collections.Generic;

namespace DB_Test;

public partial class Game
{
	public Game()
	{
		Rounds = new HashSet<Round>();
	}

	public int Id { get; set; }
	public int? Player1Id { get; set; }
	public int? Player2Id { get; set; }
	public short Hero1 { get; set; }
	public short Hero2 { get; set; }
	public byte? Winner { get; set; }

	public virtual Hero Hero1Navigation { get; set; } = null!;
	public virtual Hero Hero2Navigation { get; set; } = null!;
	public virtual Player? Player1 { get; set; }
	public virtual Player? Player2 { get; set; }
	public virtual ICollection<Round> Rounds { get; set; }
}
