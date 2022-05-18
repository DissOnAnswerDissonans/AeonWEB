using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

namespace AeonServer;

public class ServerState
{
	private readonly IHubContext<AeonGeneralHub, AeonGeneralHub.IClient> _generalHub;
	private readonly IHubContext<AeonLobbyHub, AeonLobbyHub.IClient> _lobbyHub;
	private readonly IHubContext<AeonGameHub, AeonGameHub.IClient> _gameHub;
	private readonly Services.IBalanceProvider _balance;
	private readonly ILoggerFactory _loggerFactory;
	private readonly ILogger _logger;
	public ServerState(IHubContext<AeonGeneralHub, AeonGeneralHub.IClient> hub,
		IHubContext<AeonLobbyHub, AeonLobbyHub.IClient> lobby,
		IHubContext<AeonGameHub, AeonGameHub.IClient> gameHub,
		Services.IBalanceProvider balance, ILoggerFactory lf) {
		_generalHub = hub; _lobbyHub = lobby; _gameHub = gameHub; _balance = balance;
		_loggerFactory = lf; _logger = _loggerFactory.CreateLogger("Aeon.ServerState");
	}

	internal int Number { get; set; }
	internal Dictionary<string, Room> Rooms { get; } = new();
	internal Dictionary<string, PlayerClient> IDtoPlayers { get; } = new();
	internal Dictionary<string, GameState> Games { get; } = new();

	internal bool Connected(string id, string user)
	{
		if (string.IsNullOrEmpty(id)) { 
			_logger.LogWarning("Connected() called with null ID"); 
			return false; 
		}
		IDtoPlayers.Add(id, new PlayerClient(id, user));
		_logger.LogInformation("Player {id} connected", id);
		return true;
	}

	async internal Task<bool> Disconnected(string id)
	{
		if (id is null) {
			_logger.LogWarning("Disconnected() called with null ID");
			return false;
		}

		Room? UserRoom = IDtoPlayers[id].Room;
		LeaveRoom(id);
		IDtoPlayers.Remove(id);

		if (UserRoom is not null && !UserRoom.Status.HasFlag(RoomStatus.InGame)) {
			await NotifyRoom(UserRoom.Name);
			await UpdateRoomInfo(UserRoom.Name);
		}
		_logger.LogInformation("Player {id} disconnected", id);
		return true;
	}

	internal void CreateRoom(string roomName, string rules, string? id)
	{
		var room = new Room(roomName, GetRules(rules));
		Rooms.Add(roomName, room);
		if (id == null)
			_logger.LogInformation("Room {name} created", roomName);
		else {
			room.AddPlayer(IDtoPlayers[id]);
			_logger.LogInformation("Room {name} created with player {player}", roomName, id);
		}

		IGameRules GetRules(string rulesName) => rulesName switch {
			"SingleDebug" => new SingleTestRules(),
			"Vanilla" => new VanillaRules(),
			"Tournament" => new NewRules(),
			_ => throw new ArgumentException()
		};
	}

	internal bool JoinRoom(string roomName, string id)
	{
		Room? room = Rooms[roomName];
		if (room == null || room.Status != RoomStatus.Open) return false;
		room.AddPlayer(IDtoPlayers[id]);
		_logger.LogInformation("Player {player} joined room {room}", id, roomName);
		return true;
	}

	internal void LeaveRoom(string id)
	{
		Room? room = IDtoPlayers[id].Room;
		room?.RemovePlayer(IDtoPlayers[id]);
		_logger.LogInformation("Player {player} left room {room}", id, room?.Name);
	}

	internal void DisposeRoom(string roomName)
	{
		Room? room = Rooms[roomName];
		if (room is null) return;
		room.Players.ForEach(p => p.Room = null);
		Rooms.Remove(roomName);
		_logger.LogInformation("Room {room} disposed", roomName);
	}

	internal async Task NotifyRoom(string roomName) 
		=> await _lobbyHub.Clients.Group($"ROOM_{roomName}").RefreshRoomData(Rooms[roomName].ToFullData());

	internal async Task UpdateRoomInfo(string roomName)
		=> await _lobbyHub.Clients.All.UpdSingleRoomInList(Rooms[roomName].ToShortData());

	internal async Task StartGame(string roomName)
	{
		_logger.LogInformation("Starting game in {room}", roomName);

		Room? room = Rooms[roomName];
		if (room is null) throw new ArgumentException($"Room [{roomName}] not found", nameof(roomName));
		var s = new GameState(room, _gameHub, _balance, _loggerFactory);
		Games.Add(roomName, s);
		room.SetInGame(s);

		await _generalHub.RoomClients(room).StartGame(room.ToFullData());
	}
}
