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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aeon.WindowsClient.Views;
/// <summary>
/// Interaction logic for Login.xaml
/// </summary>
public partial class Login : Page
{
	public Login()
	{
		InitializeComponent();
		AnimTest.HeroName = _vs[Random.Shared.Next(_vs.Count)];
		AnimTest.Move(3000, 3).Stop(300).Attack(500).Stop(300).Attack(500).Move(1000).Die(500).StartAnim();
	}

	private void PasswordChanged(object sender, RoutedEventArgs e)
	{
		if (DataContext != null) {
			((ViewModels.SignInVM) DataContext).Password = ((PasswordBox) sender).Password;
		}
		AnimTest.Move(2000);
	}

	private void ConfPasswordChanged(object sender, RoutedEventArgs e)
	{
		if (DataContext != null) {
			((ViewModels.SignInVM) DataContext).ConfirmPassword = ((PasswordBox) sender).Password;
		}
	}

	private List<string> _vs = new () {
		":Banker",
		":Beast",
		":BloodyElf",
		":Cheater",
		":Fatty",
		":Fe11",
		":Killer",
		":Master",
		":Rogue",
		":Thief",
		":Tramp",
		":Trickster",
		":Vampire",
		":Warlock",
		":Warrior",
	};
}
