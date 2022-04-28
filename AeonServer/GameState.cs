using Aeon.Core;

namespace AeonServer;

public class GameState
{
	private Player _player1, _player2;
	private Game _game;

	public GameState(Hero hero1, Hero hero2)
	{
		_player1 = new Player(hero1);
		_player2 = new Player(hero2);
		_game = new Game(_player1, _player2);
	}
}
