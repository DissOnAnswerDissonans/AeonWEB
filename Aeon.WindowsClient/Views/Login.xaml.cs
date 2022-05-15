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
/// Interaction logic for Login.xaml
/// </summary>
public partial class Login : Page
{
	public Login()
	{
		InitializeComponent();
		AnimTest.Move(2000).ThenStop(500).ThenAttack(1000).ThenDie(1000);
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
}
