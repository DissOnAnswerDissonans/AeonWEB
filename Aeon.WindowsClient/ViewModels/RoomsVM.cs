using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.WindowsClient.ViewModels;

internal class RoomsVM : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public ObservableCollection<Room> Rooms { get; set; }
}

internal class Room
{
	public string RoomName { get; set; }
	public int PlayersCount { get; set; }
	public int MaxPlayers { get; set; }

}
