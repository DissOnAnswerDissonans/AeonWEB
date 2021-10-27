using System;
using System.Collections.Generic;

namespace Aeon.Core
{
    public class Game : IBattle.IBattlersProv
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

		public int Battle(out Battle battle, in IBattle.ILogger logger)
		{
			battle = new Battle(this, logger);
			var winner = battle.Start();
			Player1.End(winner == 1);
			Player2.End(winner == 2);
			if (Player1.IsWinner) WinStatus = WinCond.First;
			if (Player2.IsWinner) WinStatus = WinCond.Second;
			return winner;
		}

		public Hero GetHero(int playerID) => playerID switch {
			1 => Player1.Hero,
			2 => Player2.Hero,
			_ => throw new ArgumentException("", nameof(playerID))
		};

		//int Start()
		//{
		//	while (!Player1.IsWinner && !Player2.IsWinner) {
		//		Player1.Shopping();
		//		Player2.Shopping();
		//		var battle = new Battle(this);
		//		var winner = battle.Start();
		//		Player1.End(winner == 1);
		//		Player2.End(winner == 2);		
		//	}
		//	return Player1.IsWinner	? 1 : Player2.IsWinner ? 2 : 0;
		//}

		public IEnumerable<IBattler> GetBattlers()
		{
			yield return Player1.Hero;
			yield return Player2.Hero;
			yield break;
		}
	}
}
