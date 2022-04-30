using Aeon.Base;
using Aeon.WindowsClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Aeon.WindowsClient.ViewModels;

internal class RoomsVM : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public ObservableCollection<RoomShortData> Rooms { get; set; } = new();
	public RoomShortData? SelectedRoom => NotInRoom? null : Rooms.Where(r => r.RoomName == ActiveRoomName).First();
	public string? ActiveRoomName => ActiveRoom?.RoomName;
	public RoomFullData? ActiveRoom { get; set; }

	public bool NotInRoom => ActiveRoomName is null;

	public string NewRoomName { get; set; } = "";

	public string PlayerName => App.Account.NickName;

	public RoomsVM()
	{
		App.Inst.RefreshRoomData += RefreshRoom;
	}

	private void RefreshRoom(RoomFullData room)
	{
		ActiveRoom = room;
		Refresh.Execute();
	}

	public TrofCommand Refresh => _cmdRefresh ??= new TrofCommand(async () => {
		var rooms = await App.RequestAeon<RoomShortData[]>("GetRoomsList");
		Rooms = new(rooms);
	}, () => true);
	private TrofCommand? _cmdRefresh = null;

	public TrofCommand NewRoom => _cmdNewRoom ??= new TrofCommand(async () => {
		if (string.IsNullOrEmpty(NewRoomName)) return;
		await App.CallAeon("CreateRoom", NewRoomName);
	}, () => NotInRoom);
	private TrofCommand? _cmdNewRoom = null;

	public TrofCommand<RoomShortData> Join => _cmdJoin ??= new TrofCommand<RoomShortData>(async arg => {
		if (NotInRoom)
			await App.CallAeon("JoinRoom", arg.RoomName);
	}, arg => NotInRoom);
	private TrofCommand<RoomShortData>? _cmdJoin = null;

	public TrofCommand Leave => _cmdLeave ??= new TrofCommand(async () => {
		await App.CallAeon("LeaveRoom");
	}, () => !NotInRoom);
	private TrofCommand? _cmdLeave = null;
}