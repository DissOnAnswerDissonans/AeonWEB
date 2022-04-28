using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Aeon.WindowsClient;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	//private App() => InitializeComponent();
	//[STAThread] static void Main() => new App().Run(new MainWindow());

	public static App Inst => Current as App ?? throw new AppDomainUnloadedException();
	public static Window Window => Current.MainWindow;

	internal string? JWToken { get; private set; }
	internal string ServerUrl { get; } = @"https://localhost:2366";


	internal async Task Connect(string token)
	{
		JWToken = token;
		HubConnection aeonConnection = new HubConnectionBuilder()
				.WithUrl($@"{ServerUrl}/aeon", o => o.AccessTokenProvider = () => Task.FromResult(JWToken))
				.Build();
		await aeonConnection.StartAsync();
		SwitchWindow<RoomList>();
	}

	void SwitchWindow<T>() where T : Window, new()
	{
		var old = MainWindow;
		MainWindow = new T();
		MainWindow.Show();
		old.Close();
	}  
}
