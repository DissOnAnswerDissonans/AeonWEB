using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using Trof.Connection.Client;

namespace Aeon.WindowsClient;

class Lobby : ServerConnection
{
	public ClientTx<string, string> CreateRoom { get; } = null!;
	public ClientTx<string> JoinRoom { get; } = null!;
	public ClientTx LeaveRoom { get; } = null!;
	public ClientTx ReadyCheck { get; } = null!;

	public ClientReq<RoomShortData[]> GetRoomsList { get; } = null!;
	public ClientReq<string, ClientData> GetPlayersList { get; } = null!;

	public ClientRx<RoomFullData> RefreshRoomData { get; } = null!;
	public ClientRx<RoomShortData> UpdSingleRoomInList { get; } = null!;
}
