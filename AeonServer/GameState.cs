using Aeon.Core;

namespace AeonServer;

public class GameState
{
	private List<Player> _players;
	private IGameRules _rules;
	//private Game? _game;

	public P Phase { get; private set; }

	public GameState(Room room)
	{
		_players = room.Players;
		_rules = new VanillaRules();
		Phase = P.Init;
	}

	public enum P { Init, Pick, Shop, Battle, End }

	internal void Pick()
	{
		if (Phase != P.Init) return;
		Phase = P.Pick;
	}
}

public interface IGameRules
{
	public int MinPlayers { get; }
	public int MaxPlayers { get; }

	public List<Battle> GetBattles(IEnumerable<Player> players);
}

public class VanillaRules : IGameRules
{
	public int MinPlayers => 2;
	public int MaxPlayers => 2;

	public List<Battle> GetBattles(IEnumerable<Player> players) => throw new NotImplementedException();
}

public class NewRules : IGameRules
{
	public int MinPlayers => 2;
	public int MaxPlayers => 8;

	public List<Battle> GetBattles(IEnumerable<Player> players) => throw new NotImplementedException();
}