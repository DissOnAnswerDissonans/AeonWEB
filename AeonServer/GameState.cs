using Aeon.Core;
using Microsoft.AspNetCore.SignalR;

namespace AeonServer;

public class GameState
{
	private List<Player> _players;
	public IReadOnlyList<Player> Players => _players;
	private IGameRules _rules;

	private readonly IHubContext<AeonGameHub, AeonGameHub.IClient> _gameHub;
	private Services.IBalanceProvider _balance;

	public string Name { get; }
	public P Phase { get; private set; }
	public int RoundNumber { get; private set; } = 0;
	public static Random RNG { get; } = new Random();

	public string SRGroup => $"GAME_{Name}";

	internal CancellationTokenSource? ShopCTS { get; set; }
	internal DateTimeOffset ShopCloseTime { get; private set; }

	public GameState(Room room, IHubContext<AeonGameHub, AeonGameHub.IClient> hub, Services.IBalanceProvider balance)
	{
		Name = room.Name;
		_players = room.Players;
		_rules = room.Rules;
		_balance = balance;
		_gameHub = hub;
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
		Battles = _rules.GetBattles(this).ToList(), //DEBUG
		Wage = _rules.GetBaseWage(this)
	};


	internal async Task GameStart()
	{
		await Task.Delay(3_000);
		while (true) {
			ShopCloseTime = DateTimeOffset.UtcNow.AddSeconds(30);
			Task? s = NewRoundStart();
			ShopCTS = new CancellationTokenSource();
			await Task.WhenAll(s, Timer(ShopCloseTime, ShopCTS.Token));

			foreach(Player p in _players.Where(p => p.LastShopUpdate?.Response != ShopUpdate.R.Closed))
				await _gameHub.Clients.Users(p.ID).ShopUpdated(p.GetShopUpdate(ShopUpdate.R.Closed));

			await Task.Delay(100);

			IEnumerable<Task> tasks = _rules.GetBattles(this).Select(x => StartBattle(x));
			await Task.WhenAll(tasks);
			await Task.Delay(3_000);
		}
	}

	internal static async Task Timer(DateTimeOffset t, CancellationToken token)
	{
		try {		
			await Task.Delay((int) t.Subtract(DateTimeOffset.UtcNow).TotalMilliseconds, token);
		} catch (TaskCanceledException) {
			await Task.CompletedTask;
		}
	}

	private async Task NewRoundStart()
	{
		NewRound();
		await _gameHub.Clients.Group($"GAME_{Name}").NewRound(GetRound());
		await StartShopping();
	}

	private async Task StartShopping()
	{
		foreach (Player player in Players) {
			ShopUpdate upd = player.GetShopUpdate(ShopUpdate.R.Opened);
			await _gameHub.Clients.User(player.ID).ShopUpdated(upd);
		}
	}


	private async Task StartBattle(RoundInfo.Battle battle)
	{
		Player p1 = _players.Find(p => p.ID == battle.First.PlayerName)!;
		Player p2 = _players.Find(p => p.ID == battle.Second.PlayerName)!;
		var logger = new BattleLogger(this, p1, p2);
		foreach (Battle.BattleState state in Battle(p1, p2, logger))
		{
			int delayMS = state.TurnType switch {
				Aeon.Core.Battle.TurnType.InitState => 2000,
				Aeon.Core.Battle.TurnType.AfterDamage => 500,
				Aeon.Core.Battle.TurnType.AfterHealing => 500,
				Aeon.Core.Battle.TurnType.AfterBattle => 0,
				_ => 0,
			};
			await _gameHub.Clients.User(p1.ID).NewBattleTurn(MakeTurn(state, p1, p2, delayMS));
			await _gameHub.Clients.User(p2.ID).NewBattleTurn(MakeTurn(state, p2, p1, delayMS));	
			await Task.Delay(delayMS);

			if (state.TurnType == Aeon.Core.Battle.TurnType.AfterBattle) {
				switch (state.Winner) {
					case 1: p1.Hero!.Wage(battle.Prize); break;
					case 2: p2.Hero!.Wage(battle.Prize); break;
					default: break;
				}
				p1.Hero!.Wage(_rules.GetBaseWage(this));
				p2.Hero!.Wage(_rules.GetBaseWage(this));
			}
		}
		await MulticastRoundSummary();
	}

	private static BattleTurn MakeTurn(Battle.BattleState state, Player player, Player enemy, int delay)
	{
		return new BattleTurn {
			TurnNumber = state.TurnNumber,
			TurnType = state.TurnType switch {
				Aeon.Core.Battle.TurnType.InitState => BattleTurn.T.Init,
				Aeon.Core.Battle.TurnType.AfterDamage => BattleTurn.T.Attack,
				Aeon.Core.Battle.TurnType.AfterHealing => BattleTurn.T.Heal,
				Aeon.Core.Battle.TurnType.AfterBattle => BattleTurn.T.End,
				_ => (BattleTurn.T) state.TurnType
			},
			Hero = new BattleHero {
				HeroId = player.HeroName!,
				Health = player.Hero!.StatsRO.DynConvert("HP"),
				MaxHealth = player.Hero!.StatsRO.Convert("HP"),
				ExpectedDamage = player.Hero!.ExpectedDamage,
				ExpectedCrit = player.Hero!.ExpectedCrit,
				BoostBonus = (float) player.Hero!.StatsRO.DynConvertAsIs("INC")
			},
			Enemy = new EnemyHero {
				HeroId = enemy.HeroName!,
				Health = enemy.Hero!.StatsRO.DynConvert("HP"),
				MaxHealth = enemy.Hero!.StatsRO.Convert("HP"),
			},
			NextTurnAfterMS = delay,
		};
	}

	async private Task MulticastRoundSummary()
	{
		var summary = new RoundScoreSummary {
			RoundNumber = RoundNumber,
			IsGameOver = false,
			Entries = _rules.GetScores(_players).Select(x => new RoundScoreSummary.Entry { 
				HeroName = x.Player.HeroName, Player = x.Player.ID, Score = x.Score
			}).ToList()
		};
		var u = _players.Select(p => p.ID).ToList();
		await _gameHub.Clients.Users(u).NewRoundSummary(summary);
	}

	private static IEnumerable<Battle.BattleState> Battle(Player p1, Player p2, Battle.ILogger logger)
	{
		var battle = new Battle(p1.Hero, p2.Hero, logger);

		foreach (Battle.BattleState state in battle) {
			yield return state;
			if (state.TurnType == Aeon.Core.Battle.TurnType.AfterBattle) yield break;
		}
	}

	public class BattleLogger : Battle.ILogger
	{
		private GameState _game;
		private Player _p1, _p2;
		public BattleLogger(GameState game, Player player1, Player player2) { _game = game; _p1 = player1; _p2 = player2; }

		public void LogBattleResult(int totalTurns, int winnerNumber)
			=> _game._rules.LogBattleResult(_p1, _p2, winnerNumber, totalTurns);

		public void LogBattlersState(IBattler battler1, IBattler battler2, Battle.TurnType logType) { }

		public void LogDamage(Damage dmg1to2, Damage dmg2to1) { }
	}
}