using System;
using System.Threading.Tasks;

namespace Aeon.Core
{
	public class Player
	{
		const int WAGE_WINNER = 120;
		const int WAGE_LOSER = 100;
		const int WAGE_START = 100;
		const int TARGET_WINS = 5;

		public int Score { get; private set; }
		public bool IsWinner => Score >= TARGET_WINS;

		public Hero Hero { get; }


		public Player(Hero hero)
		{
			Score = 0;
			Hero = hero;
			Hero.Wage(WAGE_START);
		}

		public Player() : this(new Hero()) { }

		internal int End(bool isWin) => isWin ? Win() : Lose();

		internal int Win()
		{
			Score++;
			Hero.Wage(WAGE_WINNER);
			return Score;
		}

		internal int Lose()
		{
			Hero.Wage(WAGE_LOSER);
			return Score;
		}

		internal void Shopping()
		{

		}
	}
}