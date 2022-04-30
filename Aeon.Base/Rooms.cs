using System;
using System.Collections.Generic;
using System.Text;

namespace Aeon.Base
{
	public class RoomShortData
	{
		public string RoomName { get; set; } = "[room]";
		public int PlayersCount { get; set; } = 0;
		public int? MaxPlayers { get; set; } = 0;
		public string PlayersStr => $"{PlayersCount}/{(MaxPlayers.HasValue ? MaxPlayers.ToString() : "∞")}";
		public RoomStatus Status { get; set; }
	}

	public class RoomFullData
	{
		public string RoomName { get; set; } = "[room]";
		public List<PlayerData> Players { get; set; }
		public int? MaxPlayers { get; set; } = 0;
		public string PlayersStr => $"{Players.Count}/{(MaxPlayers.HasValue ? MaxPlayers.ToString() : "∞")}";
		public RoomStatus Status { get; set; } = RoomStatus.Open;
		public DateTimeOffset? Countdown { get; set; } = null;
	}

	[Flags] public enum RoomStatus 
	{ 
		Open	= 0x00,
		Full	= 0x01,
		Closed	= 0x02,
		InGame	= 0x04,

		Countdown=0x40,
		Blocked = 0x80
	}

	public class PlayerData
	{
		public string PlayerName { get; set; }
		public bool IsObserver { get; set; }
		public bool IsReady { get; set; }
	}
}
