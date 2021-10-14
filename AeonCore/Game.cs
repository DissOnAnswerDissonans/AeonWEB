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

		int Start()
		{
			while (!Player1.IsWinner && !Player2.IsWinner) {
				// Закуп 1 [асинх?]
				// Закуп 2 [асинх?]
				// Бой
			}
			return Player1.IsWinner	? 1 : Player2.IsWinner ? 2 : 0;
		}

		static void Main(string[] args)
		{ 
            Console.WriteLine("Hello Aeon!");

			Player player1 = new Player(new Hero());
			Player player2 = new Player(new Hero());

			Instance = new(player1, player2);
			//Instance.Start();
        }
    }
}
