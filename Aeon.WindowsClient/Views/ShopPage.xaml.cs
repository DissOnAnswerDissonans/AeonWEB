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

namespace Aeon.WindowsClient.Views;
/// <summary>
/// Interaction logic for ShopPage.xaml
/// </summary>
public partial class ShopPage : Page
{
	ShopPageVM VM { get; }
	public ShopPage(RoundInfo r)
	{
		InitializeComponent();
		VM = (ShopPageVM) DataContext;
		VM.Round = r;

		App.Game.ShopUpdated.Subscribe(VM.OnShopUpd);
		if (App.Game.LastShopUpdate is not null) 
			VM.OnShopUpd(App.Game.LastShopUpdate);
	}
}
