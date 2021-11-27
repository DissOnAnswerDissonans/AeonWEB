using System;
using System.Collections.Generic;

namespace DB_Test;

public partial class Round
{
	public int Id { get; set; }
	public int GameId { get; set; }
	public short Number { get; set; }

	public virtual Game Game { get; set; } = null!;
}
