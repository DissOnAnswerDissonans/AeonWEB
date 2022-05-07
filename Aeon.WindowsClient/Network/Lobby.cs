using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Reactive.Linq;
using AeonServer;

namespace Aeon.WindowsClient;

class Lobby : ServerConnection
{
	public Lobby(string? token, string url) : base(token, $"{url}/aeon/lobby") 
	{
		RefreshRoomData = Observable.Create<RoomFullData>
			(obs => Connection.On<RoomFullData>("RefreshRoomData", s => obs.OnNext(s)));
		UpdSingleRoomInList = Observable.Create<RoomShortData>
			(obs => Connection.On<RoomShortData>("UpdSingleRoomInList", s => obs.OnNext(s)));
	}

	public IObservable<RoomFullData> RefreshRoomData { get; }
	public IObservable<RoomShortData> UpdSingleRoomInList { get; }

	public async Task CreateRoom(string roomName) => await Call("CreateRoom", roomName);
	public async Task JoinRoom(string roomName) => await Call("JoinRoom", roomName);
	public async Task LeaveRoom() => await Call("LeaveRoom");
	public async Task ReadyCheck() => await Call("ReadyCheck");
	public async Task<RoomShortData[]> GetRoomsList() => await Request<RoomShortData[]>("GetRoomsList");
	public async Task<PlayerData[]> GetPlayersList(string room) => await Request<PlayerData[]>("GetPlayersList", room);
}
