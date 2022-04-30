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
using System.Windows.Media;

namespace Aeon.WindowsClient.ViewModels;

internal class RoomListVM : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public ObservableCollection<RoomVM> Rooms { get; set; } = new();
	public ObservableCollection<PlayerVM> PlayersInRoom 
		=> new(ActiveRoom?.Players.Select(p => new PlayerVM(p)) ?? new List<PlayerVM>());

	public string? ActiveRoomName => ActiveRoom?.RoomName;
	public RoomFullData? ActiveRoom { get; set; }

	public bool NotInRoom => ActiveRoomName is null;

	public string NewRoomName { get; set; } = "";

	public string PlayerName => App.Account.NickName;

	public string ReadyText => ActiveRoom?.Countdown?.ToString() ?? "Ready";

	public TrofCommand Refresh => _cmdRefresh ??= new TrofCommand(async () => {
		var rooms = await App.RequestAeon<RoomShortData[]>("GetRoomsList");
		Rooms = new(rooms.Select(r => new RoomVM(r) { IsSelected = ActiveRoomName == r.RoomName }));
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
		NewRoomName = ActiveRoomName!;
	}, arg => NotInRoom);
	private TrofCommand<RoomShortData>? _cmdJoin = null;

	public TrofCommand Leave => _cmdLeave ??= new TrofCommand(async () => {
		await App.CallAeon("LeaveRoom");
		NewRoomName = "";
	}, () => !NotInRoom);
	private TrofCommand? _cmdLeave = null;

	public TrofCommand Disconnect => _cmdDisconnect ??= new TrofCommand(async () => {
		await App.Inst.Disconnect();
	}, () => true);
	private TrofCommand? _cmdDisconnect = null;

	public TrofCommand Ready => _cmdReady ??= new TrofCommand(async () => {
		await App.CallAeon("ReadyCheck");
	}, () => true);
	private TrofCommand? _cmdReady = null;


}

public class PlayerVM
{
	public PlayerData Data { get; }
	public PlayerVM(PlayerData data) => Data = data;
	public string ReadyDig => Data.IsReady ? "X" : "-";

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class RoomVM
{
	public RoomShortData Data { get; }
	public RoomVM(RoomShortData data) => Data = data;
	public bool IsAvailiable => !Data.Status.HasFlag(RoomStatus.Countdown);
	public bool IsFull => Data.Status.HasFlag(RoomStatus.Full);
	public bool IsClosedRoom => Data.Status.HasFlag(RoomStatus.Closed);
	public bool IsSelected { get; set; }
	public Brush NameColor => IsAvailiable ? colorOpen : colorClosed;
	public Brush AmountColor => !IsFull ? colorOpen : colorClosed;
	public Brush SelectColor => IsSelected ? selected : empty;

	public static readonly Brush colorOpen = new SolidColorBrush(new(){ R = 0, G = 64, B = 0, A = 255 });
	public static readonly Brush colorClosed = new SolidColorBrush(new(){ R = 192, G = 0, B = 0, A = 255 });
	public static readonly Brush empty = new SolidColorBrush(new(){ R = 0, G = 0, B = 0, A = 0 });
	public static readonly Brush selected = new SolidColorBrush(new(){ R = 0, G = 0, B = 0, A = 32 });

	public event PropertyChangedEventHandler? PropertyChanged;
}