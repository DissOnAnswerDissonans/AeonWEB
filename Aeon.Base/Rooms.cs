using System;
using System.Collections.Generic;
using System.Text;

namespace Aeon.Base
{
	public class RoomShortData
	{
		public string RoomName { get; set; } = "[room]";
		public int PlayersCount { get; set; } = 0;
		public int MinPlayers { get; set; } = 2;
		public int? MaxPlayers { get; set; } = null;
		public string PlayersStr => $"{PlayersCount}/{(MaxPlayers.HasValue ? MaxPlayers.ToString() : "∞")}";
		public RoomStatus Status { get; set; }
	}

	public class RoomFullData : RoomShortData
	{
		public List<ClientData> Players { get; set; }
		public DateTimeOffset? Countdown { get; set; } = null;
	}

	[Flags] public enum RoomStatus 
	{ 
		Open	= 0x00,
		Full	= 0x01,
		Closed	= 0x02,
		InGame	= 0x04,

		Disposing=0x20,
		Countdown=0x40,
		Blocked = 0x80
	}

	public class ClientData
	{
		public string PlayerName { get; set; }
		public bool IsObserver { get; set; }
		public bool IsReady { get; set; }
	}
}
