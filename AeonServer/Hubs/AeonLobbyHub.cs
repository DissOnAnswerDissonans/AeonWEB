using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Aeon.Base;
using System.Diagnostics;
using Aeon.Core;

namespace AeonServer;


[Authorize]
public class AeonLobbyHub : AeonHub<AeonLobbyHub.IClient>
{
	private readonly ServerState _state;
	public AeonLobbyHub(ServerState state)
	{
		_state = state;
	}

	private string? UserRoomName => UserRoom?.Name;
	private Room? UserRoom => _state.IDtoPlayers[UserID].Room;

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		if (UserRoomName is not null && !_state.Rooms[UserRoomName].Status.HasFlag(RoomStatus.InGame)) {
			await NotifyRoom(UserRoomName);
			await UpdateRoomInfo(UserRoomName);
		}
		await base.OnDisconnectedAsync(exception);
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
		if (_state.JoinRoom(roomName, UserID)) {
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
		_state.LeaveRoom(UserID);
		await Clients.Caller.RefreshRoomData(null);
		await NotifyRoom(temp);
		await UpdateRoomInfo(temp);
		return;
	}

	public async Task ReadyCheck()
	{
		if (UserRoom is null) return;
		if (UserRoom.Players.Count < UserRoom.NeedPlayers) return;
		_state.IDtoPlayers[UserID].Data.IsReady = !_state.IDtoPlayers[UserID].Data.IsReady;
		if (UserRoom.Players.All(p => p.Data.IsReady)) {	
			_ = UserRoom.SetCountdown(2, async r => await _state.StartGame(r.Name));
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



	private async Task NotifyRoom(string room) =>
		await Clients.Group($"ROOM_{room}").RefreshRoomData(_state.Rooms[room].ToFullData());

	private async Task UpdateRoomInfo(string room)
		=> await Clients.All.UpdSingleRoomInList(_state.Rooms[room].ToShortData());

	public interface IClient
	{
		Task RefreshRoomData(RoomFullData? room);
		Task UpdSingleRoomInList(RoomShortData room);
	}
}
