namespace DB_Test;

public partial class Buy
{
	public int GameId { get; set; }
	public byte RoundNumber { get; set; }

	public byte StatId { get; set; }
	public bool IsOpt { get; set; }
	public byte Amount { get; set; }
	public byte PlayerNumber { get; set; }

	public virtual Round Round { get; set; } = null!;
	public virtual Stat Stat { get; set; } = null!;
}
