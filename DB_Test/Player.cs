namespace DB_Test;

public partial class Player
{
	public int Id { get; set; }
	public string Nickname { get; set; } = null!;
	public int? Pwhash { get; set; }
	public decimal? ValueElo { get; set; }

	public virtual ICollection<Game> GamesOnPos1 { get; set; } = new HashSet<Game>();
	public virtual ICollection<Game> GamesOnPos2 { get; set; } = new HashSet<Game>();
}
