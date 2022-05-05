using System;
using System.Collections.Generic;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AeonCore.Tests")]

namespace Aeon.Core
{
	public class Game
	{
		public enum WinCond { Undecided, First, Second, None = -1 }

		public Player Player1 { get; }
		public Player Player2 { get; }

		public static Random RNG { get; } = new Random();

		public WinCond WinStatus { get; private set; } = WinCond.Undecided;

		public Game(Player pl1, Player pl2)
		{
			Player1 = pl1;
			Player2 = pl2;
		}

		async public IAsyncEnumerable<Battle.BattleState> Battle(Battle.ILogger logger)
		{
			var battle = new Battle(Player1.Hero, Player2.Hero, logger);

			foreach (Battle.BattleState state in battle) {
				yield return state;
				if (state.TurnType == Core.Battle.TurnType.AfterBattle) yield break;
				await System.Threading.Tasks.Task.Delay(500);
			}
		}

		public Hero GetHero(int playerID) => playerID switch {
			1 => Player1.Hero,
			2 => Player2.Hero,
			_ => throw new ArgumentException("", nameof(playerID))
		};
	}
}