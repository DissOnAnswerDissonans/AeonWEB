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
using static Aeon.WindowsClient.ViewModels.ShopPageVM;

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

		App.Game.ShopUpdated.On(VM.OnShopUpd);
		App.Game.ShopUpdated.On(SetTimer);
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

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		var offer = (OfferVM) (sender as Button)!.DataContext;
		App.Game.BuyOffer.Send(offer.Offer.ID);
	}

	private void EndShoppingButton(object sender, RoutedEventArgs e)
	{
		App.Game.DoneShopping.Send();
	}

	private void HeroButton_Click(object sender, RoutedEventArgs e)
	{
		VM.SelectedPosition = null;
	}

	private void AbilityButton_Click(object sender, RoutedEventArgs e)
	{

	}
}
