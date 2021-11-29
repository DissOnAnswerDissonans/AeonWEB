namespace DB_Test;

public partial class Game
{
	public int Id { get; set; }

	public int? Player1Id { get; set; }
	public virtual Player? Player1 { get; set; }

	public int? Player2Id { get; set; }
	public virtual Player? Player2 { get; set; }

	public short Hero1Id { get; set; }
	public virtual Hero Hero1 { get; set; } = null!;

	public short Hero2Id { get; set; }
	public virtual Hero Hero2 { get; set; } = null!;

	public byte? Winner { get; set; }


	public virtual ICollection<Round> Rounds { get; set; } = new HashSet<Round>();
}
