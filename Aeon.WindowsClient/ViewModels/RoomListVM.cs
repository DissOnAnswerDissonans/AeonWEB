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
	public ObservableCollection<RoomSlot> RoomSlots { get {
			if (ActiveRoom is null) return new();

			List<RoomSlot> list = ActiveRoom.Players.Select(p => new RoomSlot(p)).ToList();
			for (int plN = list.Count; plN < ActiveRoom.MaxPlayers; ++plN) {
				list.Add(plN < ActiveRoom.MinPlayers ? RoomSlot.Need : RoomSlot.Open);
			}
			return new(list);
		}
	}

	public string? ActiveRoomName => ActiveRoom?.RoomName;
	public RoomFullData? ActiveRoom { get; set; }

	public bool NotInRoom => ActiveRoomName is null;
	public bool IsReady => ActiveRoom?.Players.Where(p => p.PlayerName == PlayerName).FirstOrDefault()?.IsReady ?? false;

	public string NewRoomName { get; set; } = "";
	public string SelectedMode { get; set; } = "Vanilla";

	public ObservableCollection<string> Modes { get; } = new(){ "SingleDebug", "Vanilla", "Tournament" };

	public string PlayerName => App.Account.NickName;

	public string ReadyText => ActiveRoom?.PlayersCount < ActiveRoom?.MinPlayers ? "Waiting for players…"
		: ActiveRoom?.Countdown is null ? "Ready" : "Game is starting…";

	public TrofCommand Refresh => _cmdRefresh ??= new TrofCommand(async () => {
		var rooms = await App.Lobby.GetRoomsList.Request();
		Rooms = new(rooms.Select(r => new RoomVM(r) { IsSelected = ActiveRoomName == r.RoomName }));
	}, () => true);
	private TrofCommand? _cmdRefresh = null;

	public TrofCommand NewRoom => _cmdNewRoom ??= new TrofCommand(() => {
		if (string.IsNullOrEmpty(NewRoomName))
			NewRoomName = $"{SelectedMode}_{Random.Shared.Next(1000, 10000)}";
		App.Lobby.CreateRoom.Send(NewRoomName, SelectedMode);
	}, () => NotInRoom);
	private TrofCommand? _cmdNewRoom = null;

	public TrofCommand<RoomShortData> Join => _cmdJoin ??= new TrofCommand<RoomShortData>(arg => {
		if (NotInRoom)
			App.Lobby.JoinRoom.Send(arg.RoomName);
		NewRoomName = ActiveRoomName!;
	}, arg => NotInRoom);
	private TrofCommand<RoomShortData>? _cmdJoin = null;

	public TrofCommand Leave => _cmdLeave ??= new TrofCommand(() => {
		App.Lobby.LeaveRoom.Send();
		NewRoomName = "";
	}, () => !NotInRoom && !IsReady);
	private TrofCommand? _cmdLeave = null;

	public TrofCommand Disconnect => _cmdDisconnect ??= new TrofCommand(async () => {
		await App.Inst.Disconnect();
	}, () => !IsReady);
	private TrofCommand? _cmdDisconnect = null;

	public TrofCommand Ready => _cmdReady ??= new TrofCommand(() => {
		App.Lobby.ReadyCheck.Send();
	}, () => !NotInRoom && ActiveRoom?.PlayersCount >= ActiveRoom?.MinPlayers);
	private TrofCommand? _cmdReady = null;
}

public class RoomSlot
{
	public string Name { get; set; } = "";
	public string ReadySignal { get; set; } = "";
	private RoomSlot() { }
	public RoomSlot(ClientData p)
	{
		Name = p.PlayerName;
		ReadySignal = p.IsReady ? "X" : "-";
	}
	public static RoomSlot Need => new() { Name = "{need}" };
	public static RoomSlot Open => new() { Name = "(open)" };

	public Brush NickBrush => (Brush) App.Inst.FindResource(Name switch {
		"{need}" => "Rooms_Need",
		"(open)" => "Rooms_Open",
		_ => "TextColor"
	});
}

public class RoomVM
{
	public RoomShortData Data { get; }
	public RoomVM(RoomShortData data) => Data = data;
	public bool IsAvailiable => !Data.Status.HasFlag(RoomStatus.Countdown);
	public bool IsFull => Data.Status.HasFlag(RoomStatus.Full);
	public bool IsClosedRoom => Data.Status.HasFlag(RoomStatus.Closed);
	public bool IsSelected { get; set; }
	public Brush NameColor => (Brush)
		App.Inst.FindResource(IsAvailiable ? "Rooms_Open" : "Rooms_Closed");
	public Brush AmountColor => (Brush)
		App.Inst.FindResource(IsFull ? "Rooms_Closed" 
			: Data.PlayersCount < Data.MinPlayers? "Rooms_Need"
			: "Rooms_Open");
	public Brush SelectColor => IsSelected ? selected : empty;

	public static readonly Brush empty = new SolidColorBrush(new(){ R = 0, G = 0, B = 0, A = 0 });
	public static readonly Brush selected = new SolidColorBrush(new(){ R = 0, G = 255, B = 255, A = 32 });

	public event PropertyChangedEventHandler? PropertyChanged;
}