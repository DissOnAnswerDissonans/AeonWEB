﻿using Aeon.Base;
using Aeon.WindowsClient.ViewModels;
using AeonServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aeon.WindowsClient.Views;
/// <summary>
/// Interaction logic for RoomList.xaml
/// </summary>
public partial class RoomList : Page
{
	RoomListVM VM { get; }
	public RoomList()
	{
		InitializeComponent();
		VM = (RoomListVM) DataContext; 
		VM.Refresh.Execute();

		App.Lobby.RefreshRoomData.On(RefreshRoom);
		App.Lobby.UpdSingleRoomInList.On(UpdSingleRoom);

		CommandManager.InvalidateRequerySuggested();
	}

	private void RefreshRoom(RoomFullData room)
	{
		VM.ActiveRoom = room;
		CommandManager.InvalidateRequerySuggested();
	}

	private void UpdSingleRoom(RoomShortData obj)
	{
		RoomVM? room = VM.Rooms.Where(r => r.Data.RoomName == obj.RoomName).FirstOrDefault();
		if (room is null)
			VM.Rooms.Add(new RoomVM(obj) { IsSelected = VM.ActiveRoomName == obj.RoomName });
		else if (obj.Status.HasFlag(RoomStatus.Disposing))
			VM.Rooms.Remove(room);
		else
			VM.Rooms[VM.Rooms.IndexOf(room)] = new RoomVM(obj) { IsSelected = VM.ActiveRoomName == obj.RoomName };
		CommandManager.InvalidateRequerySuggested();
	}

	private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (e.AddedItems.Count > 0)
			(DataContext as RoomListVM)?.Join.Execute((e.AddedItems[0] as RoomVM)!.Data);
		CommandManager.InvalidateRequerySuggested();
	}

	private void RemoveBot(object sender, RoutedEventArgs e) => App.Lobby.RemoveBot.Send();
	private void AddBot(object sender, RoutedEventArgs e) => App.Lobby.AddBot.Send();
}
