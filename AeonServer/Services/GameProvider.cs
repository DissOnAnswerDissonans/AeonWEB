namespace AeonServer.Services;

public class GameProvider
{
	private readonly ServerState _state;
	public GameProvider(ServerState state) => _state = state;

	public GameState FromUser(string username) => _state.IDtoPlayers[username].Game!;
	public GameState FromRoom(string roomName) => _state.Games[roomName];

	public PlayerClient GetPlayer(string userName) => _state.IDtoPlayers[userName];

	public TotalInfoDownload GetTotalInfo(string userName)
	{
		GameState game = FromUser(userName)!;
		return new TotalInfoDownload {
			Balance = new(),
			HeroesList = new(),
			StatsList = new()
		};
	}
}
