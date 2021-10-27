using System;
using System.Collections;
using Aeon.Core;
using DrawingCLI;

namespace Aeon.BasicApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.ResetColor();
			Print.Pos(3, 1, "Hello Aeon!");
			DrawTitle();
			Console.ReadKey();


			Player player1 = new Player(new Heroes.Cheater());
			Player player2 = new Player(new Heroes.Fatty());



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
