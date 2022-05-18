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
	public PositionVM SelectedPosition { get; set; }
	public int SelectedID { get; set; } = 0;
	public int Money { get; set; }
	public string MoneyText => $"Денег {Money}₽";


	internal void OnShopUpd(ShopUpdate upd)
	{
		var id = SelectedID;
		Money = upd.Hero.Money;
		Positions = new(upd.Hero.Stats
			.GroupJoin(upd.Offers, stat => stat.StatId, offer => offer.StatAmount.StatId, (stat, offers) =>
				new PositionVM { Stat = stat, Name = stat.StatId.ToString(), Offers = offers.Select(o => new OfferVM(this, o)).ToList() })
			.Where(x => x.Offers.Any()));
		SelectedPosition = Positions[id];
	}

	internal class PositionVM
	{
		public StatData Stat { get; set; }
		public string Name { get; set; }
		public List<OfferVM> Offers { get; set; }
		public Visibility ConvertedVis => Stat.RawValue == Stat.Value ? Visibility.Collapsed : Visibility.Visible;
	}

	internal class OfferVM
	{
		private ShopPageVM _ctx;
		public OfferVM(ShopPageVM shopPage, OfferData offer) { _ctx = shopPage; Offer = offer; }

		public OfferData Offer { get; }
		public string OfferText => $"{Offer.StatAmount.RawValue} for {Offer.Cost}";
		public bool Opt => Offer.IsOpt;
		public bool IsAvailiable => _ctx.Money >= Offer.Cost;
	}
}
