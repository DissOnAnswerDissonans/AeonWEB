using System.Collections.Generic;
using System.Linq;

namespace Aeon.Core;

public class Battle
{
	private readonly IBattler _h1;
	private readonly IBattler _h2;
	private readonly ILogger _logger;

	public Battle(IBattler battler1, IBattler battler2, ILogger logger = null)
	{
		_h1 = battler1;
		_h2 = battler2;
		_logger = logger;
	}

	public int Rounds { get; private set; } = 0;

	public int Winner { get; private set; } = 0;

	public IEnumerator<BattleState> GetEnumerator()
	{
		_h1.OnBattleStart(_h2);
		_h2.OnBattleStart(_h1);

		const int MAX_ROUNDS = 50;

		yield return Log(TurnType.InitState);

		while (_h1.IsAlive && _h2.IsAlive && Rounds < MAX_ROUNDS) {
			++Rounds;

			Damage dmg1to2 = _h1.GetDamageTo(_h2);
			Damage dmg2to1 = _h2.GetDamageTo(_h1);

			Damage received1 = _h1.ReceiveDamage(dmg2to1);
			Damage received2 = _h2.ReceiveDamage(dmg1to2);

			_logger?.LogDamage(received2, received1);
			yield return Log(TurnType.AfterDamage);

			if (!(_h1.IsAlive && _h2.IsAlive)) break;

			_h1.AfterHit(received1, received2);
			_h2.AfterHit(received2, received1);

			yield return Log(TurnType.AfterHealing);
		}

		Winner = _h1.IsAlive ? 1 : _h2.IsAlive ? 2 : 0;

		_h1.AfterBattle(_h2, Winner == 1);
		_h2.AfterBattle(_h1, Winner == 2);

		yield return Log(TurnType.AfterBattle);
		_logger?.LogBattleResult(Rounds, Winner);
		yield break;
	}

	BattleState Log(TurnType turnType)
	{
		_logger?.LogBattlersState(_h1, _h2, turnType);
		return new BattleState {
			TurnNumber = Rounds, TurnType = turnType, Winner = Winner,
			Battlers = new[] { _h1, _h2 }
		};
	}

	public class BattleState
	{
		public int TurnNumber { get; set; }
		public TurnType TurnType { get; set; }
		public IBattler[] Battlers { get; set; }
		public int Winner { get; set; } = -1;
	}

	public enum TurnType { InitState, AfterDamage, AfterHealing, AfterBattle }

	public interface ILogger
	{
		void LogBattlersState(IBattler battler1, IBattler battler2, TurnType logType);

		void LogDamage(Damage dmg1to2, Damage dmg2to1);

		void LogBattleResult(int totalTurns, int winnerNumber);
	}
}
