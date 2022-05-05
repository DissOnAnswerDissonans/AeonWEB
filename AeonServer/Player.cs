using Aeon.Core;

namespace AeonServer;

public class Player
{
	public Room? Room { get; internal set; }
	public GameState? Game { get; internal set; }
	public PlayerData Data { get; internal set; }
	public Hero? Hero { get; private set; }
	
	public Player(string nickname)
	{
		Room = null;
		Data = new PlayerData { PlayerName = nickname, IsObserver = false, IsReady = false };
	}

	internal void SelectHero(Hero hero) => Hero = hero;
}