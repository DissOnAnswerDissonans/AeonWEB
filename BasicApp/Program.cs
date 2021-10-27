using System;
using System.Collections;
using Aeon.Core;
using Aeon.Heroes;
using DrawingCLI;

namespace Aeon.BasicApp
{
	class Program
	{
		public static readonly Colors[] PlayerColors  = new Colors[] {
			new() { Color = ConsoleColor.Blue, BGColor = ConsoleColor.Black },
			new() { Color = ConsoleColor.DarkYellow, BGColor = ConsoleColor.Black }
		};

		static void Main(string[] args)
		{
			Console.ResetColor();
			Print.Pos(3, 1, "Hello Aeon!");
			DrawTitle();
			Console.ReadKey();

			PickPresenter pick = new();

			Player player1 = new Player(pick.PickHero(0));
			Player player2 = new Player(pick.PickHero(1));

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

		public static void DrawTitle()
		{
			var bytes = new byte[256];
			for (int i = 0; i < 256; ++i) bytes[i] = (byte) i;
			ColorPic pic = new(32, 8, bytes);
			pic.DrawIn(24, 6);
		}
	}
}
