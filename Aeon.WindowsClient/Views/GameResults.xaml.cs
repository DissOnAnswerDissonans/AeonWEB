using Aeon.WindowsClient.ViewModels;
using AeonServer.Models;
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
using static Aeon.WindowsClient.Views.HeroDisplay;

namespace Aeon.WindowsClient.Views;
/// <summary>
/// Interaction logic for GameResults.xaml
/// </summary>
public partial class GameResults : Page
{
	GameResultsVM VM { get; }
	public GameResults(FinalResult result)
	{
		InitializeComponent();
		VM = (GameResultsVM) DataContext;
		VM.Result = result;

		winner.Move(750, 1, Dir.E).Attack(500, Dir.NE).Attack(500, Dir.E).Attack(500, Dir.SE).Move(750, 1, Dir.S).StartAnim();
		loser.Move(2250, 3, Dir.W).Die(750, Dir.SW).StartAnim();
	}

	private void Exit_Click(object sender, RoutedEventArgs e)
	{
		_ = App.Inst.LeaveGame();
	}
}
