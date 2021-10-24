﻿using System;
using System.Threading.Tasks;

namespace Aeon.Core
{
	public class Player
	{
		const int WAGE_WINNER = 120;
		const int WAGE_LOSER = 100;

		public int Score { get; private set; }
		public bool IsWinner => Score >= 5;

		public Hero Hero { get; }


		public Player(Hero hero)
		{
			Score = 0;
			Hero = hero;
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