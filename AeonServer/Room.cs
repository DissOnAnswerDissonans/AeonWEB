namespace AeonServer;

public class Room
{
	public string Name { get; }
	public List<string> Players { get; } = new();
	public LinkedList<string> Observers { get; } = new();

	public GameState? GameState { get; }

	internal S Status { get; set; }
	internal enum S { Waiting, Ready, InGame }

	internal Room(string name, string? player)
	{
		Name = name;
		if (player is not null)
			Players.Add(player);
	}
}
