using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Aeon.Base;

namespace AeonServer;

[Authorize]
public class AeonHub : Hub//<AeonHub.IClient>
{
	private readonly ServerState _state;
	public AeonHub(ServerState state) => _state = state;

	private string UserName => Context.User?.Identity?.Name!;

	public override Task OnConnectedAsync()
	{
		_state.Number++;
		_state.Connected(UserName);
		return base.OnConnectedAsync();
	}

	public override Task OnDisconnectedAsync(Exception? exception)
	{
		LeaveRoom().Wait();
		_state.Number--;
		_state.Disconnected(UserName);
		return base.OnDisconnectedAsync(exception);
	}

	public async Task<AccountInfo> GetAccountInfo()
	{
		return await Task.FromResult(new AccountInfo { NickName = UserName });
	}

	public async Task CreateRoom(string roomName)
	{
		await Groups.AddToGroupAsync(Context.ConnectionId, $"ROOM_{roomName}");
		_state.CreateRoom(roomName, null);
		await JoinRoom(roomName);
		return;
	}

	public async Task JoinRoom(string roomName)
	{
		await Groups.AddToGroupAsync(Context.ConnectionId, $"ROOM_{roomName}");
		_state.JoinRoom(roomName, UserName);
		await NotifyRoom(roomName);
		return;
	}

	public async Task LeaveRoom()
	{
		var roomName = _state.Users[UserName]?.Name;
		if (roomName is null) return;
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ROOM_{roomName}");
		_state.LeaveRoom(UserName);
		await Clients.Caller.SendAsync("RefreshRoomData", null);
		await NotifyRoom(roomName);
		return;
	}

	public async Task<RoomShortData[]> GetRoomsList() 
		=> await Task.FromResult(_state.Rooms.Select(r => new RoomShortData {
			RoomName = r.Key, 
			PlayersCount = r.Value.Players.Count, 
			MaxPlayers = null 
		}).ToArray());

	public async Task<string[]> GetPlayersList(string room)
		=> await Task.FromResult(_state.Rooms[room].Players.ToArray());

	public async Task NotifyRoom(string room) =>
		await Clients.Group($"ROOM_{room}").SendAsync("RefreshRoomData", new RoomFullData {
			RoomName = room,
			Players = _state.Rooms[room].Players,
			MaxPlayers = null
		});

	//public interface IClient
	//{
	//	Task RefreshRoom(RoomFullData roomData);
	//	Task SetToRoom(string? room);
	//}
}

