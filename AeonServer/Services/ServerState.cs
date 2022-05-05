namespace AeonServer;

public class ServerState
{
	internal int Number { get; set; }
	internal Dictionary<string, Room> Rooms { get; } = new();
	internal Dictionary<string, Player> Players { get; } = new();
	internal Dictionary<string, GameState> Games { get; } = new();

	internal bool Connected(string user)
	{
		if (user is null) return false;
		Players.Add(user, new Player(user));
		return true;
	}

	internal bool Disconnected(string user)
	{
		if (user is null) return false;
		LeaveRoom(user);
		Players.Remove(user);
		return true;
	}

	internal void CreateRoom(string roomName, string? user)
	{
		var room = new Room(roomName);
		Rooms.Add(roomName, room);
		if (user == null) return;
		room.AddPlayer(Players[user]);
	}

	internal bool JoinRoom(string roomName, string user)
	{
		Room? room = Rooms[roomName];
		if (room == null || room.Status != RoomStatus.Open) return false;
		room.AddPlayer(Players[user]);
		return true;
	}

	internal void LeaveRoom(string user)
	{
		Players[user].Room?.RemovePlayer(Players[user]);
	}

	internal void DisposeRoom(string roomName)
	{
		Room? room = Rooms[roomName];
		if (room is null) return;
		room.Players.ForEach(p => p.Room = null);
		Rooms.Remove(roomName);
	}

	internal void StartGame(string roomName)
	{
		Room? room = Rooms[roomName];
		if (room is null) return;
		Games.Add(roomName, new GameState(room));
	}
}
