using Aeon.WindowsClient.ViewModels;
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
/// Interaction logic for BattleView.xaml
/// </summary>
public partial class BattleView : Page
{
	private BattleVM VM;
	private BattleTurn? _lastTurn;

	public BattleView()
	{
		InitializeComponent();
		VM = (BattleVM) DataContext;
		App.Game.NewBattleTurn.Subscribe(OnNewTurn);
	}

	private void OnNewTurn(BattleTurn turn)
	{
		if (_lastTurn is not null) {
			ShowEffect(_lastTurn.Hero.Health - turn.Hero.Health, false);
			ShowEffect(_lastTurn.Enemy.Health - turn.Enemy.Health, true);
		}
		_lastTurn = turn;
		VM.BattleTurn = turn;

	}

	private void ShowEffect(int change, bool isEnemy)
	{
		
	}
}
