global using Aeon.Base;

using Aeon.WindowsClient.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Reactive;
using AeonServer.Models;
using System.IO;

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

	internal string? ServerUrl { get; private set; }

	internal static AeonGeneral General { get; private set; } = null!;
	internal static Lobby Lobby { get; private set; } = null!;
	internal static Game Game { get; private set; } = null!;
	

	internal static AccountInfo Account { get; private set; } = null!;

	internal async Task Connect(string url, string? token)
	{
		ServerUrl = url;
		General = new(token, url);
		Lobby = new(token, url);
		Game = new(token, url);

		General.OnGameStart.Subscribe(async d => await Start(d));

		Game.NewRoundStarted.Subscribe(async r => await NewRound(r));
		Game.ShopUpdated.Subscribe(u => { if (u.Response == ShopUpdate.R.Closed) SwitchPage<BattleView>(); });

		Game.GameOver.Subscribe(x => SwitchPage<GameResults>(x));

		await General.Connect();
		await Lobby.Connect();

		Account = await General.GetAccountInfo();
		SwitchPage<RoomList>();
	}

	internal async Task Disconnect()
	{
		SwitchPage<Login>();
		await D();
	}

	internal async Task LeaveGame()
	{
		await Game.LeaveGame();
		await Game.Disconnect();
		await Lobby.Connect();
		SwitchPage<RoomList>();
	}

	private async Task D()
	{
		if (General is null) return;
		await General.Disconnect();
		await Lobby.Disconnect();
	}

	private async Task Start(RoomFullData data)
	{
		await Game.Connect();
		SwitchPage<HeroSelect>();
		await Lobby.Disconnect();
	}

	private Task NewRound(RoundInfo r)
	{
		SwitchPage<ShopPage>(r);
		return Task.CompletedTask;
	}

	private T SwitchPage<T>() where T : Page, new()
	{
		var window = new T();
		Window.Content = window;
		return window;
	}

	private T SwitchPage<T>(object data) where T : Page
	{
		var window = (T) Activator.CreateInstance(typeof(T), data)!;
		Window.Content = window;
		return window;
	}

	protected override void OnExit(ExitEventArgs e)
	{
		_ = D();
		base.OnExit(e);
	}
}