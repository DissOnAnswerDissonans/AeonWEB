namespace DB_Test;

public partial class Round
{
	public byte Number { get; set; }
	public int GameId { get; set; }
	public byte Winner { get; set; }

	public virtual Game Game { get; set; } = null!;

	public virtual ICollection<Attack> Attacks { get; set; } = new HashSet<Attack>();
}
