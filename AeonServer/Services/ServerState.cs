using Microsoft.AspNetCore.SignalR;

namespace AeonServer;

public class ServerState
{
	private readonly IHubContext<AeonGeneralHub, AeonGeneralHub.IClient> _generalHub;
	private readonly IHubContext<AeonGameHub, AeonGameHub.IClient> _gameHub;
	private readonly Services.IBalanceProvider _balance;
	public ServerState(IHubContext<AeonGeneralHub, AeonGeneralHub.IClient> hub,
		IHubContext<AeonGameHub, AeonGameHub.IClient> gameHub,
		Services.IBalanceProvider balance) {
		_generalHub = hub; _gameHub = gameHub; _balance = balance;
	}

	internal int Number { get; set; }
	internal Dictionary<string, Room> Rooms { get; } = new();
	internal Dictionary<string, Player> IDtoPlayers { get; } = new();
	internal Dictionary<string, GameState> Games { get; } = new();

	internal bool Connected(string id, string user)
	{
		if (id is null) return false;
		IDtoPlayers.Add(id, new Player(id, user));
		return true;
	}

	internal bool Disconnected(string id)
	{
		if (id is null) return false;
		LeaveRoom(id);
		IDtoPlayers.Remove(id);
		return true;
	}

	internal void CreateRoom(string roomName, string? id)
	{
		var room = new Room(roomName, new VanillaRules());
		Rooms.Add(roomName, room);
		if (id == null) return;
		room.AddPlayer(IDtoPlayers[id]);
	}

	internal bool JoinRoom(string roomName, string id)
	{
		Room? room = Rooms[roomName];
		if (room == null || room.Status != RoomStatus.Open) return false;
		room.AddPlayer(IDtoPlayers[id]);
		return true;
	}

	internal void LeaveRoom(string id)
	{
		IDtoPlayers[id].Room?.RemovePlayer(IDtoPlayers[id]);
	}

	internal void DisposeRoom(string roomName)
	{
		Room? room = Rooms[roomName];
		if (room is null) return;
		room.Players.ForEach(p => p.Room = null);
		Rooms.Remove(roomName);
	}

	internal async Task StartGame(string roomName)
	{
		Room? room = Rooms[roomName];
		if (room is null) throw new ArgumentException($"Room [{roomName}] not found", nameof(roomName));
		var s = new GameState(room, _gameHub, _balance);
		Games.Add(roomName, s);
		room.SetInGame(s);

		await _generalHub.RoomClients(room).StartGame(room.ToFullData());
	}
}
