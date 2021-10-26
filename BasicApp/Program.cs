using System;
using Aeon.Core;

namespace Aeon.BasicApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello Aeon!");

			Player player1 = new Player();
			Player player2 = new Player();

			Game game = new Game(player1, player2);
			ShopPresenter shop = new(game);
			BattlePresenter battle = new(game);

			while (game.WinStatus == Game.WinCond.Undecided) {
				shop.EnterShopRoutine(1);
				shop.EnterShopRoutine(2);
				battle.StartBattle();
			}

			Console.Clear();
			Console.WriteLine($"Winner: {game.WinStatus}");
			Console.Write("Goodbye Aeon!");
		}
	}

	class BattlePresenter : IBattle.ILogger
	{
		private Game _game;

		public Battle LastBattle { get; private set; }

		public BattlePresenter(Game game)
		{
			_game = game;
		}

		public void StartBattle()
		{
			_game.Battle(out Battle battle, this);
			LastBattle = battle;
		}

		public void LogBattlersState(IBattler battler1, IBattler battler2)
		{
			// UNDONE
		}
	}
}
