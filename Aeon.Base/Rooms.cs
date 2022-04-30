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
	}

	public class RoomFullData
	{
		public string RoomName { get; set; } = "[room]";
		public List<string> Players { get; set; }
		public int? MaxPlayers { get; set; } = 0;
		public string PlayersStr => $"{Players.Count}/{(MaxPlayers.HasValue ? MaxPlayers.ToString() : "∞")}";
	}
}
