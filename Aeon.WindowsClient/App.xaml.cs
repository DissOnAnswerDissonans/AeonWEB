global using Aeon.Base;

using Aeon.WindowsClient.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Reactive;

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

	internal string ServerUrl { get; } = @"https://localhost:2366";

	internal static AeonGeneral General { get; private set; } = null!;
	internal static Lobby Lobby { get; private set; } = null!;
	

	internal static AccountInfo Account { get; private set; } = null!;

	internal async Task Connect(string? token)
	{
		General = new(token, ServerUrl);
		Lobby = new(token, ServerUrl);
		await General.Connect();
		await Lobby.Connect();

		Account = await General.GetAccountInfo();
		SwitchPage<RoomList>();
	}

	internal async Task Disconnect()
	{
		SwitchPage<Login>();
		await General.Disconnect();
		await Lobby.Disconnect();
	}

	private void SwitchPage<T>() where T : Page, new()
	{
		(Window.Content as IDisposable)?.Dispose();
		Window.Content = new T();
	}

	protected override void OnExit(ExitEventArgs e)
	{
		_ = Disconnect();
		base.OnExit(e);
	}
}
