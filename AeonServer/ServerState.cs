namespace AeonServer;

public class ServerState
{
	internal int Number { get; set; }
	internal Dictionary<string, Room> Rooms { get; } = new();

	internal Dictionary<string, Room?> Users { get; } = new();

	internal bool Connected(string user)
	{
		if (user is null) return false;
		Users.Add(user, null);
		return true;
	}

	internal void CreateRoom(string roomName, string user)
	{
		var room = new Room(roomName, user);
		Rooms.Add(roomName, room);
		Users[user] = room;
	}

	internal void JoinRoom(string roomName, string user)
	{
		var room = Rooms[roomName];
		room.Observers.AddLast(user);
		Users[user] = room;
	}

	internal void LeaveRoom(string user)
	{
		Users[user]?.Observers.Remove(user);
		Users[user]?.Players.Remove(user);
		Users[user] = null;
	}
}
