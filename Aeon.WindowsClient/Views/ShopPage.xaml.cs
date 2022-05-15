using Aeon.WindowsClient.ViewModels;
using AeonServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
		App.Game.ShopUpdated.Subscribe(SetTimer);
		if (App.Game.LastShopUpdate is not null) 
			VM.OnShopUpd(App.Game.LastShopUpdate);
	}

	private void SetTimer(ShopUpdate upd)
	{
		if (timer.Time == 0) {
			TimeSpan t = upd.CloseIn - DateTimeOffset.UtcNow;
			double time = t.TotalSeconds;
			timer.SetTime(time);
		}
		if (upd.Response == ShopUpdate.R.Closed)
			timer.SetTime(0);
	}

	private async void Button_Click(object sender, RoutedEventArgs e)
	{
		var offer = (OfferData) (sender as Button)!.DataContext;
		await App.Game.BuyOffer(offer.ID);
	}

	private async void EndShoppingButton(object sender, RoutedEventArgs e)
	{
		await App.Game.DoneShopping();
	}
}
