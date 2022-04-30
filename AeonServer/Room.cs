using Aeon.Base;
using Aeon.Core;

namespace AeonServer;

public class Room
{
	public string Name { get; }
	public List<Player> Players { get; } = new();
	public int? RoomSize { get; private set; } = 2;
	public RoomStatus Status { get; private set; } = RoomStatus.Open;

	private DateTimeOffset? _timer = null;
	private CancellationTokenSource? _cts = null;

	public GameState? GameState { get; }

	public bool IsFull => Players.Count >= RoomSize;

	internal Room(string name)
	{
		Name = name;
	}

	internal bool AddPlayer(Player player)
	{
		if (IsFull || Players.Contains(player)) return false;
		player.Room?.RemovePlayer(player);
		player.Room = this;
		Players.Add(player);
		if (IsFull)
			Status |= RoomStatus.Full;
		return true;
	}

	internal bool RemovePlayer(Player player)
	{
		int index = Players.IndexOf(player);
		if (index == -1) return false;
		Players.RemoveAt(index);
		player.Room = null;
		Status &= ~RoomStatus.Full;
		return true;
	}

	internal async Task SetCountdown(double seconds, Action? action)
	{
		Status |= RoomStatus.Countdown;
		_cts = new CancellationTokenSource();
		_timer = DateTimeOffset.UtcNow.AddSeconds(seconds);
		await Task.Delay(TimeSpan.FromSeconds(seconds), _cts.Token);
		_timer = null;
		if (!_cts.IsCancellationRequested) action?.Invoke();
		_cts.Dispose();
		_cts = null;
	}
	internal void ResetCountdown()
	{
		Status &= ~RoomStatus.Countdown;
		_timer = null;
		_cts?.Cancel();
	} 

	public class Player
	{
		public Room? Room { get; internal set; }
		public PlayerData Data { get; internal set; }
		public Player (string nickname)
		{
			Room = null;
			Data = new PlayerData { PlayerName = nickname, IsObserver = false, IsReady = false };
		}
	}

	internal RoomShortData ToShortData() => new() { 
		RoomName = Name, PlayersCount = Players.Count, MaxPlayers = RoomSize, Status = Status
	};

	internal RoomFullData ToFullData() => new() {
		RoomName = Name, Players = Players.Select(p => p.Data).ToList(), 
		MaxPlayers = RoomSize, Status = Status, Countdown = _timer
	};
}
