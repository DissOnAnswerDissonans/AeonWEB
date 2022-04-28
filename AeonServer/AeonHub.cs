using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AeonServer;

[Authorize]
public class AeonHub : Hub<AeonHub.IClient>
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
		_state.Number--;
		return base.OnDisconnectedAsync(exception);
	}


	public async Task CreateRoom(string roomName)
	{
		await Groups.AddToGroupAsync(Context.ConnectionId, $"ROOM_{roomName}");
		_state.CreateRoom(roomName, UserName);
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
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ROOM_{_state.Users[UserName]?.Name}");
		_state.LeaveRoom(UserName);
		return;
	}

	public async Task<string[]> GetRoomsList() 
		=> await Task.FromResult(_state.Rooms.Keys.ToArray());

	public async Task<string[]> GetPlayersList(string room)
		=> await Task.FromResult(_state.Rooms[room].Players.ToArray());

	public async Task NotifyRoom(string room) =>
		await Clients.Group($"ROOM_{room}").RefreshRoom();

	public interface IClient
	{
		Task RefreshRoom();
		//Task RefreshRoomList();
	}
}

