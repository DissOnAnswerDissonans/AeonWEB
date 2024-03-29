﻿using Aeon.Core;
using Microsoft.AspNetCore.SignalR;

namespace AeonServer;

public class GameState
{
	public IReadOnlyList<PlayerClient> Clients => _clients;
	private List<PlayerClient> _clients;
	private List<PlayerBot> _bots;
	public IReadOnlyList<Player> Players => _clients.Cast<Player>().Concat(_bots).ToList();

	//public IReadOnlyList<Player> Players => _clients;
	//private List<Player> _clients;
	private IGameRules _rules;

	private readonly IHubContext<AeonGameHub, AeonGameHub.IClient> _gameHub;
	private Services.IBalanceProvider _balance;
	private ILogger _logger;

	public string Name { get; }
	public P Phase { get; private set; }
	public int RoundNumber { get; private set; } = 0;
	public static Random RNG { get; } = new Random();

	public string SRGroup => $"GAME_{Name}";

	internal CancellationTokenSource CTS { get; }
	internal CancellationTokenSource? ShopCTS { get; set; }
	internal DateTimeOffset ShopCloseTime { get; private set; }

	public GameState(Room room, IHubContext<AeonGameHub, AeonGameHub.IClient> hub, 
		Services.IBalanceProvider balance, ILoggerFactory loggerFactory)
	{
		Name = room.Name;
		_clients = room.Players.OfType<PlayerClient>().ToList(); // говнокод
		_bots = room.Players.OfType<PlayerBot>().ToList();
		_rules = room.Rules;
		_balance = balance;
		_gameHub = hub;
		_logger = loggerFactory.CreateLogger($"Aeon.GameState.{room.Name}");
		Phase = P.Init;
		CTS = new();
	}

	public enum P { Init, Pick, Shop, Battle, End }

	internal void Pick(HeroInfo[] heroes, Services.HeroesProvider _heroes)
	{
		if (Phase != P.Init) return;
		Phase = P.Pick;
		_logger.LogInformation("Pick started");
		_bots.ForEach(b => b.AutoSelectHero(heroes, _heroes));
	}

	internal void NewRound()
	{
		RoundNumber++;
		Phase = P.Shop;
		Players.Select(p => p.Hero).ToList().ForEach(h => h?.OnRoundStart());
		_logger.LogInformation("Round {number} started", RoundNumber);
	}

	internal RoundInfo GetRound() => new RoundInfo {
		Number = RoundNumber,
		Battles = _rules.GetBattles(this).ToList(), //DEBUG
		Wage = _rules.GetBaseWage(this)
	};

	internal async Task GameStart()
	{
		_logger.LogInformation("Game is ready to start");
		_rules.BeforeGame(this);
		await Task.Delay(3_000, CTS.Token);
		while (_rules.GetWinner(this) is null) {
			ShopCloseTime = DateTimeOffset.UtcNow.AddSeconds(30);
			Task? s = NewRoundStart();
			ShopCTS = new CancellationTokenSource();
			await Task.WhenAll(s, Timer(ShopCloseTime, ShopCTS.Token));

			foreach(PlayerClient p in _clients.Where(p => p.LastShopUpdate?.Response != ShopUpdate.R.Closed))
				await _gameHub.Clients.Users(p.ID).ShopUpdated(p.GetShopUpdate(ShopUpdate.R.Closed));

			_logger.LogInformation("Starting battles...");

			await Task.Delay(100, CTS.Token);

			var b = _rules.GetBattles(this);

			IEnumerable<Task> tasks = b.Select(x => StartBattle(x));
			await Task.WhenAll(tasks);
			await Task.Delay(3_000, CTS.Token);
		}
		var result = FinalResult;
		_logger.LogWarning("Game over! Winner is {winner}", result.Winner);
		await _gameHub.Clients.Group($"GAME_{Name}").GameOver(result);
	}

	internal void PlayerLeft(PlayerClient player)
	{
		_logger.LogWarning("Player {p} left", player.ID);
		_clients.Remove(player);
		//_bots.Add();
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
		await _gameHub.Clients.Group($"GAME_{Name}").NewRoundStarted(GetRound());
		await StartShopping();
	}

	private async Task StartShopping()
	{
		foreach (PlayerClient player in Clients) {
			ShopUpdate upd = player.GetShopUpdate(ShopUpdate.R.Opened);
			await _gameHub.Clients.User(player.ID).ShopUpdated(upd);
		}
		foreach (PlayerBot bot in _bots)
			bot.AutoBuy();
	}


	private async Task StartBattle(RoundInfo.Battle battle)
	{
		Player p1 = Players.Where(p => p.ID == battle.First.PlayerName).First();
		Player p2 = Players.Where(p => p.ID == battle.Second.PlayerName).First();
		var logger = new BattleLogger(this, p1, p2, _logger);
		_logger.LogInformation("Battle [{p1} vs {p2}] started", p1, p2);

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
		_logger.LogInformation("Battle [{p1} vs {p2}] ended", p1, p2);
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
		var u = _clients.Select(p => p.ID).ToList();
		await _gameHub.Clients.Users(u).NewRoundSummary(RoundScoreSummary);
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
		private ILogger _logger;
		public BattleLogger(GameState game, Player player1, Player player2, ILogger logger) 
		{ _game = game; _p1 = player1; _p2 = player2; _logger = logger; }

		public void LogBattleResult(int totalTurns, int winnerNumber)
		{
			_game._rules.LogBattleResult(_p1, _p2, winnerNumber, totalTurns);
			_logger.LogTrace("[{p1} vs {p2}]: Got battle result", _p1?.ID, _p2?.ID);
		}

		public void LogBattlersState(IBattler battler1, IBattler battler2, Battle.TurnType logType) 
		{
			_logger.LogDebug("[{p1} vs {p2}]: Turn {turn}" + "\n\t" +
				"P1 ({h1}) => HP: {hp1}, INC: {inc1}" + "\n\t" +
				"P2 ({h2}) => HP: {hp2}, INC: {inc2}", _p1?.ID, _p2?.ID, logType,
				battler1.ID, battler1.StatsRO.GetDynValue("HP"), battler1.StatsRO.DynConvertAsIs("INC"),
				battler2.ID, battler2.StatsRO.GetDynValue("HP"), battler2.StatsRO.DynConvertAsIs("INC")
			);
		}

		public void LogDamage(Damage dmg1to2, Damage dmg2to1)
		{
			_logger.LogDebug("\t" +
				"[{p1} => {p2}]: {dmg}" + "\n\t" +
				"[{p1} <= {p2}]: {dmg}", _p1?.ID, _p2?.ID, dmg1to2, _p1?.ID, _p2?.ID, dmg2to1);
		}
	}


	public RoundScoreSummary RoundScoreSummary => new() {
		RoundNumber = RoundNumber,
		Entries = _rules.GetScores(Players).Select(x => new RoundScoreSummary.Entry {
			HeroName = x.Player.HeroName, Player = x.Player.ID, Score = x.Score
		}).ToList()
	};

	public FinalResult FinalResult => new() {
		Scores = RoundScoreSummary,
		Winner = _rules.GetWinner(this)!.ID,
		Players = Players.ToDictionary(p => p.ID, p => Converters.FromAeon(p.Hero!))
	};
}