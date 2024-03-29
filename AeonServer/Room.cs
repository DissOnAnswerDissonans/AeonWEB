﻿using Aeon.Base;
using Aeon.Core;
using Microsoft.AspNetCore.SignalR;

namespace AeonServer;

public class Room
{
	public string Name { get; }
	public IGameRules Rules { get; }
	public List<Player> Players { get; } = new();
	public int NeedPlayers => Rules.MinPlayers;
	public int? RoomSize => Rules.MaxPlayers;
	public int TotalBots { get; private set; } = 0;
	public RoomStatus Status { get; private set; } = RoomStatus.Open;

	private DateTimeOffset? _timer = null;
	private CancellationTokenSource? _cts = null;

	public GameState? Game { get; private set; }

	public bool IsFull => Players.Count >= RoomSize;

	internal Room(string name, IGameRules rules)
	{
		Name = name;
		Rules = rules;
	}

	internal bool AddPlayer(PlayerClient player)
	{
		if (IsFull || Players.Contains(player)) return false;
		player.Room?.RemovePlayer(player);
		player.Room = this;
		Players.Add(player);
		if (IsFull)
			Status |= RoomStatus.Full;
		return true;
	}

	internal bool RemovePlayer(PlayerClient player)
	{
		int index = Players.IndexOf(player);
		if (index == -1) return false;
		Players.RemoveAt(index);
		player.Room = null;
		Status &= ~RoomStatus.Full;
		return true;
	}

	internal bool AddBot()
	{
		if (IsFull) return false;
		TotalBots++;
		Players.Add(new PlayerBot($"B{TotalBots}", this));
		if (IsFull)
			Status |= RoomStatus.Full;
		return true;
	}

	internal bool RemoveBot()
	{
		Player? bot = Players.Where(p => p.ID.StartsWith("<BOT>") 
				&& p.ID.Contains($"B{TotalBots}")).FirstOrDefault();
		if (bot is null) return false;
		Players.Remove(bot);
		TotalBots--;
		Status &= ~RoomStatus.Full;
		return true;
	}

	internal async Task SetCountdown(double seconds, Action<Room>? action)
	{
		Status |= RoomStatus.Countdown;
		_cts = new CancellationTokenSource();
		_timer = DateTimeOffset.UtcNow.AddSeconds(seconds);
		await Task.Delay(TimeSpan.FromSeconds(seconds), _cts.Token);
		_timer = null;
		if (!_cts.IsCancellationRequested) action?.Invoke(this);
		_cts.Dispose();
		_cts = null;
	}
	internal void ResetCountdown()
	{
		Status &= ~RoomStatus.Countdown;
		_timer = null;
		_cts?.Cancel();
	}

	internal void SetInGame(GameState game)
	{
		Status |= RoomStatus.InGame;
		Game = game;
	}

	internal async Task SetDisposing()
	{
		Status |= RoomStatus.Disposing | RoomStatus.Blocked;
		await Task.Delay(10);
		Game?.CTS.Cancel();
		Players.ForEach(p => p.Room = null);
		Players.Clear();
	}

	internal RoomShortData ToShortData() => new() { 
		RoomName = Name, PlayersCount = Players.Count, MinPlayers = NeedPlayers, MaxPlayers = RoomSize, Status = Status
	};

	internal RoomFullData ToFullData() => new() {
		RoomName = Name, Players = Players.Select(p => p.Data).ToList(), PlayersCount = Players.Count,
		MinPlayers = NeedPlayers, MaxPlayers = RoomSize, Status = Status, Countdown = _timer
	};
}
