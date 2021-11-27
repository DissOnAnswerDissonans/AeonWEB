using System;
using System.Collections.Generic;

namespace DB_Test;

public partial class Attack
{
	public int RoundId { get; set; }
	public short Number { get; set; }
	public string? Result { get; set; }
	public bool Crit1 { get; set; }
	public bool Crit2 { get; set; }

	public virtual Round Round { get; set; } = null!;
}
