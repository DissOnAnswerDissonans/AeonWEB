using System;

namespace AeonCore
{
	public class Player
	{
		public int Score { get; }
		public bool IsWinner => Score >= 5;

		public Hero Hero { get; }

		
		public Player(Hero hero)
		{
			Score = 0;
			Hero = hero;
		}
	}
}