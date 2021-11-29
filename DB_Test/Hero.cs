namespace DB_Test;

public partial class Hero
{
	public short Id { get; set; }
	public string AsmName { get; set; } = null!;

	public virtual ICollection<Game> GamesPick1 { get; set; } = new HashSet<Game>();
	public virtual ICollection<Game> GamesPick2 { get; set; } = new HashSet<Game>();
}
