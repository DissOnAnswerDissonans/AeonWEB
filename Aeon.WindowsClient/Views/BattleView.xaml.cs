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
using System.Windows.Media.Animation;

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
		App.Game.NewBattleTurn.On(OnNewTurn);
		App.Game.NewRoundSummary.On(s => VM.Summary = s);
	}

	private void OnNewTurn(BattleTurn turn)
	{
		if (_lastTurn is not null) {
			ShowEffect(_lastTurn.Hero.Health - turn.Hero.Health, false);
			ShowEffect(_lastTurn.Enemy.Health - turn.Enemy.Health, true);
		}
		_lastTurn = turn;
		VM.BattleTurn = turn;

		int delay = turn.NextTurnAfterMS;

		if (turn.TurnType == BattleTurn.T.Init) {
			Hero1.Move(delay - delay/4, 2).Attack(delay / 4).StartAnim();
			Hero2.Move(delay - delay/4, 2).Attack(delay / 4).StartAnim();

			var a1 = new DoubleAnimation(-200, 0, TimeSpan.FromMilliseconds(delay - delay/4));
			var a2 = new DoubleAnimation(+200, 0, TimeSpan.FromMilliseconds(delay - delay/4));

			Hero1.RenderTransform.BeginAnimation(TranslateTransform.XProperty, a1);
			Hero2.RenderTransform.BeginAnimation(TranslateTransform.XProperty, a2);
		}
		if (turn.TurnType == BattleTurn.T.Heal) {
			Hero1.Attack(delay).StartAnim();
			Hero2.Attack(delay).StartAnim();
		}
		if (turn.TurnType == BattleTurn.T.End) {
			if (turn.Hero.Health <= 0)  Hero1.Die(1000).StartAnim();
			if (turn.Enemy.Health <= 0) Hero2.Die(1000).StartAnim();
		}

	}

	private void ShowEffect(int change, bool isEnemy)
	{
		
	}
}
