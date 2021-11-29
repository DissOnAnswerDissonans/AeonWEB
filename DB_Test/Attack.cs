namespace DB_Test;

public partial class Attack
{
	public int GameId { get; set; }
	public byte RoundNumber { get; set; }
	public byte Number { get; set; }

	public virtual Round Round { get; set; } = null!;

	public string? Result { get; set; }
	public bool Crit1 { get; set; }
	public bool Crit2 { get; set; }
}
