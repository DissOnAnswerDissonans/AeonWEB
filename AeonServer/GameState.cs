using Aeon.Core;

namespace AeonServer;

public class GameState
{
	private List<Player> _players;
	public IReadOnlyList<Player> Players => _players;
	private IGameRules _rules;

	private Services.IBalanceProvider _balance;

	public string Name { get; }
	public P Phase { get; private set; }
	public int RoundNumber { get; private set; } = 0;
	public static Random RNG { get; } = new Random();

	public string SRGroup => $"GAME_{Name}";

	public GameState(Room room, IGameRules rules, Services.IBalanceProvider balance)
	{
		Name = room.Name;
		_players = room.Players;
		_rules = rules;
		_balance = balance;
		Phase = P.Init;
	}

	public enum P { Init, Pick, Shop, Battle, End }

	internal void Pick()
	{
		if (Phase != P.Init) return;
		Phase = P.Pick;
	}

	internal void NewRound()
	{
		RoundNumber++;
		Phase = P.Shop;
	}

	internal RoundInfo GetRound() => new RoundInfo {
		Number = RoundNumber,
		//Battles = _rules.GetBattles(this).ToList(), //DEBUG
		//Wage = _rules.GetWage(this)
	};



	async private IAsyncEnumerable<Battle.BattleState> Battle(Player p1, Player p2, Battle.ILogger logger)
	{
		var battle = new Battle(p1.Hero, p2.Hero, logger);

		foreach (Battle.BattleState state in battle) {
			yield return state;
			if (state.TurnType == Aeon.Core.Battle.TurnType.AfterBattle) yield break;
			await System.Threading.Tasks.Task.Delay(500);
		}
	}
}

public interface IGameRules
{
	public int MinPlayers { get; }
	public int MaxPlayers { get; }

	public IEnumerable<RoundInfo.Battle> GetBattles(GameState game);
	public int GetWage(GameState game);
}

public class VanillaRules : IGameRules
{
	public int MinPlayers => 2;
	public int MaxPlayers => 2;

	public IEnumerable<RoundInfo.Battle> GetBattles(GameState game) => throw new NotImplementedException();
	public int GetWage(GameState game) => throw new NotImplementedException();
}

public class NewRules : IGameRules
{
	public int MinPlayers => 2;
	public int MaxPlayers => 8;

	public IEnumerable<RoundInfo.Battle> GetBattles(GameState game) => throw new NotImplementedException();
	public int GetWage(GameState game) => throw new NotImplementedException();
}