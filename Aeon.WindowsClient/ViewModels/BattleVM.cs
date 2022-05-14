using Aeon.Base;
using Aeon.WindowsClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace Aeon.WindowsClient.ViewModels;

internal class BattleVM : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;
	public BattleTurn? BattleTurn { get; set; }

	public Visibility EnemyWait => BattleTurn is null ? Visibility.Visible : Visibility.Hidden;
	public Visibility ScoreVisible => 
		BattleTurn?.TurnType == BattleTurn.T.End ? Visibility.Visible : Visibility.Collapsed;

	public int TurnNumber => BattleTurn?.TurnNumber ?? -1;
	public Brush TurnNumberBrush => new SolidColorBrush(BattleTurn?.TurnType switch {
		BattleTurn.T.Init => Colors.WhiteSmoke,
		BattleTurn.T.Attack => Colors.Red,
		BattleTurn.T.Heal => Colors.Green,
		BattleTurn.T.End => Colors.Gray,
		_ => Colors.Black,
	});

	public string HealthText => BattleTurn is null ? "???/???" : $"{BattleTurn?.Hero.Health}/{BattleTurn?.Hero.MaxHealth}";
	public string EHealthText => BattleTurn is null ? "???/???" : $"{BattleTurn?.Enemy.Health}/{BattleTurn?.Enemy.MaxHealth}";
}
