using System;

namespace AeonCore
{
	public class Player
	{
		public int Score { get; }

		public int Money { get; }

		public Hero Hero { get; }

		public Shop Shop { get; }

		
		public Player(Hero hero)
		{
			Score = 0;
			Money = 100;
			Hero = hero;
			Shop = new Shop();
		}
	}
}