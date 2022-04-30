using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Aeon.Base;
using System.Diagnostics;

namespace AeonServer;

[Authorize]
public class AeonHub : Hub//<AeonHub.IClient>
{
	private readonly ServerState _state;
	public AeonHub(ServerState state) => _state = state;

	private string UserName => Context.User?.Identity?.Name!;
	private string? UserRoomName => UserRoom?.Name;
	private Room? UserRoom => _state.Players[UserName].Room;

	public override Task OnConnectedAsync()
	{
		_state.Number++;
		_state.Connected(UserName);
		return base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		var name = UserRoomName;
		_state.Number--;
		_state.Disconnected(UserName);
		if (name is not null) {
			await NotifyRoom(name);
			await UpdateRoomInfo(name);
		}
		await Task.FromResult(base.OnDisconnectedAsync(exception));
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
		if (_state.JoinRoom(roomName, UserName)) {
			await Groups.AddToGroupAsync(Context.ConnectionId, $"ROOM_{roomName}");
			await NotifyRoom(roomName);
			await UpdateRoomInfo(roomName);
		} else {
			await GetRoomsList();
		}
	}

	public async Task LeaveRoom()
	{
		if (UserRoomName is null) return;
		var temp = UserRoomName;
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ROOM_{UserRoomName}");
		_state.LeaveRoom(UserName);
		await Clients.Caller.SendAsync("RefreshRoomData", null);
		await NotifyRoom(temp);
		await UpdateRoomInfo(temp);
		return;
	}

	public async Task ReadyCheck()
	{
		if (UserRoom is null) return;
		_state.Players[UserName].Data.IsReady = !_state.Players[UserName].Data.IsReady;
		if (UserRoom.Players.All(p => p.Data.IsReady)) {	
			_ = UserRoom.SetCountdown(10, SendToAeon);
		} else {
			UserRoom.ResetCountdown();
		}
		await UpdateRoomInfo(UserRoomName!);
		await NotifyRoom(UserRoomName!);
	}

	public async Task<RoomShortData[]> GetRoomsList()
		=> await Task.FromResult(_state.Rooms.Select(r => r.Value.ToShortData()).ToArray());

	public async Task<PlayerData[]> GetPlayersList(string room)
		=> await Task.FromResult(_state.Rooms[room].Players.Select(p => p.Data).ToArray());

	public async Task NotifyRoom(string room) =>
		await Clients.Group($"ROOM_{room}").SendAsync("RefreshRoomData", _state.Rooms[room].ToFullData());

	public async Task UpdateRoomInfo(string room)
		=> await Clients.All.SendAsync("UpdSingleRoomInList", _state.Rooms[room].ToShortData());

	public void SendToAeon()
	{
		Debug.WriteLine("ВСЁ");
	}

	//public interface IClient
	//{
	//	Task RefreshRoom(RoomFullData roomData);
	//	Task SetToRoom(string? room);
	//}
}

