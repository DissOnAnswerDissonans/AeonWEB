using System;

namespace AeonCore
{
    public class Game
	{
		public Player Player1 { get; }
		public Player Player2 { get; }
		public static Game Instance { get; private set; }

		private Game(Player pl1, Player pl2) 
		{
			Player1 = pl1;
			Player2 = pl2;
		}

		static void Main(string[] args)
		{ 
            Console.WriteLine("Hello Aeon!");

			Player player1 = new Player(new Hero());
			Player player2 = new Player(new Hero());

			Instance = new(player1, player2);

        }
    }
}
