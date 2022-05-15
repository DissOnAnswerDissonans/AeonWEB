using Aeon.Base;
using Aeon.WindowsClient;
using AeonServer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Aeon.WindowsClient.ViewModels;
internal class ShopPageVM : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public RoundInfo Round { get; set; } = null!;
	public ObservableCollection<PositionVM> Positions { get; set; } = new();
	public int Money { get; set; }
	public string MoneyText => $"Денег {Money}₽";
	public string TimerText { get; set; }


	internal void OnShopUpd(ShopUpdate upd)
	{
		Money = upd.Hero.Money;
		Positions = new(upd.Hero.Stats
			.GroupJoin(upd.Offers, stat => stat.StatId, offer => offer.StatAmount.StatId, (stat, offers) =>
				new PositionVM { Stat = stat, Name = stat.StatId.ToString(), Offers = offers.ToList() })
			.Where(x => x.Offers.Any()));
	}

	internal class PositionVM
	{
		public StatData Stat { get; set; }
		public string Name { get; set; }
		public List<OfferData> Offers { get; set; }
	}
}
