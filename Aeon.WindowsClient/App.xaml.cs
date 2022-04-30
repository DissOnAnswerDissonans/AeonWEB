using Aeon.Base;
using Aeon.WindowsClient.Views;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.CompilerServices;
using System.Diagnostics;

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

	internal static HubConnection Aeon { get; private set; } = null!;
	internal static AccountInfo Account { get; private set; } = null!;

	internal async Task Connect(string token)
	{
		JWToken = token;
		Aeon = new HubConnectionBuilder()
				.WithUrl($@"{ServerUrl}/aeon", o => o.AccessTokenProvider = () => Task.FromResult(JWToken))
				.Build();

		//Aeon.On<RoomFullData>("RefreshRoom", data => Dispatcher.Invoke(() => RefreshRoom?.Invoke(data)));
		//Aeon.On<string?>("SetToRoom", data => Dispatcher.Invoke(() => SetToRoom?.Invoke(data)));
		//Aeon.On<string>("Test1", str => Dispatcher.Invoke(() => Console.WriteLine(str)));

		await Aeon.StartAsync();
		Account = await RequestAeon<AccountInfo>("GetAccountInfo");
		SwitchPage<RoomList>();

		AddAeonEvent(RefreshRoomData);
	}

	void SwitchPage<T>() where T : Page, new()
	{
		Window.Content = new T();
	}

	static async internal Task<T> RequestAeon<T>(string method, object obj) => await Aeon.InvokeAsync<T>(method, obj);
	static async internal Task<T> RequestAeon<T>(string method) => await Aeon.InvokeAsync<T>(method);
	static async internal Task CallAeon(string method) => await Aeon.SendAsync(method);
	static async internal Task CallAeon(string method, object obj) => await Aeon.SendAsync(method, obj);

	public event Action<RoomFullData> RefreshRoomData = data => { };


	private void AddAeonEvent<T>(Action<T> action, [CallerArgumentExpression("action")] string method = "")
	{
		Debug.WriteLine($"---- Action {method} added ----");
		Aeon.On<T>(method, arg => Dispatcher.Invoke(() => action?.Invoke(arg)));
	}

	protected override void OnExit(ExitEventArgs e)
	{
		Aeon.StopAsync();
		base.OnExit(e);
	}
}

//internal static class HubExt
//{
//	public static IDisposable OnCall<T>(this HubConnection hub, Action<T> action)
//	{
//		return hub.On<T>(nameof(action), action);
//	}
//}