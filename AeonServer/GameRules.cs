namespace AeonServer;

public interface IGameRules
{
	public int MinPlayers { get; }
	public int MaxPlayers { get; }

	public IEnumerable<RoundInfo.Battle> GetBattles(GameState game);
	public int GetBaseWage(GameState game);
	public Player? GetWinner(GameState game);
	public int GetScore(Player player);
	public List<(Player Player, int Score)> GetScores(IReadOnlyList<Player> players);

	public void LogBattleResult(Player p1, Player p2, int winner, int turns);
	void BeforeGame(GameState gameState);
}

public class SingleTestRules : IGameRules
{
	public int MinPlayers => 1;
	public int MaxPlayers => 1;

	public void BeforeGame(GameState gameState)
	{
		gameState.AddDummy("debug");
	}

	public int GetBaseWage(GameState game) => 100;
	public IEnumerable<RoundInfo.Battle> GetBattles(GameState game) => new List<RoundInfo.Battle>() {
		new () {
			Prize = 20,
			First = game.Players[0].Contender,
			Second = game.Players[1].Contender,
		}
	};

	public int GetScore(Player player) => 0;
	public List<(Player Player, int Score)> GetScores(IReadOnlyList<Player> players) => new();
	public Player? GetWinner(GameState game) => null;
	public void LogBattleResult(Player p1, Player p2, int winner, int turns) { }
}

public class VanillaRules : IGameRules
{
	public int MinPlayers => 2;
	public int MaxPlayers => 2;

	public void BeforeGame(GameState gameState) { }

	public IEnumerable<RoundInfo.Battle> GetBattles(GameState game) => new List<RoundInfo.Battle>() {
		new() {
			Prize = 20,
			First = game.Players[0].Contender,
			Second = game.Players[1].Contender
		}
	};
	public int GetBaseWage(GameState game) => 100;
	public Player? GetWinner(GameState game) => _winner;
	public int GetScore(Player player) => (_scores.TryGetValue(player, out int value)) ? value : 0;

	public List<(Player Player, int Score)> GetScores(IReadOnlyList<Player> players)
	{
		var list = players.Select(pl => (pl, GetScore(pl))).ToList();
		list.Sort((x, y) => y.Item2.CompareTo(x.Item2));
		return list;
	}

	private Dictionary<Player, int> _scores = new();
	private Player? _winner = null;

	private int AddPointTo(Player name)
		=> _scores[name] = _scores.TryGetValue(name, out int value) ? value + 1 : 1;

	public void LogBattleResult(Player p1, Player p2, int winner, int turns)
	{
		if (winner > 0) {
			var pt = AddPointTo(winner switch { 1 => p1!, 2 => p2!, _ => throw null! });
			if (pt >= 5) _winner = _scores.Where(s => s.Value >= 5).Select(x => x.Key).FirstOrDefault();
		}
	}
}

public class NewRules : IGameRules
{
	public int MinPlayers => 2;
	public int MaxPlayers => 8;

	public void BeforeGame(GameState gameState) { }
	public IEnumerable<RoundInfo.Battle> GetBattles(GameState game) => throw new NotImplementedException();
	public int GetBaseWage(GameState game) => 100;
	public Player? GetWinner(GameState game) => throw new NotImplementedException();
	public int GetScore(Player player) => throw new NotImplementedException();


	public void LogBattleResult(Player p1, Player p2, int winner, int turns) => throw new NotImplementedException();
	public List<(Player Player, int Score)> GetScores(IReadOnlyList<Player> players) => throw new NotImplementedException();
}