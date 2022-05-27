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

	public Hero? Hero { get; set; }
	public RoundInfo Round { get; set; } = null!;
	public ObservableCollection<PositionVM> Positions { get; set; } = new();
	public ObservableCollection<HeroStatVM> OtherStats { get; set; } = new();
	public PositionVM? SelectedPosition { get; set; }
	public int SelectedID { get; set; } = 0;
	public string MoneyText => $"{Hero?.Money}₽";
	public string AbilityText { get; set; } = "No Ability";


	public Visibility StatSel => SelectedPosition switch { null => Visibility.Collapsed, _ => Visibility.Visible };
	public Visibility HeroSel => SelectedPosition switch { null => Visibility.Visible, _ => Visibility.Collapsed };


	internal void OnShopUpd(ShopUpdate upd)
	{
		var id = SelectedID;
		Hero = upd.Hero;
		Positions = new(upd.Hero.Stats
			.GroupJoin(upd.Offers, stat => stat.StatId, offer => offer.StatAmount.StatId, (stat, offers) =>
				new PositionVM { Stat = stat, Name = stat.StatId.ToString(), Offers = offers.Select(o => new OfferVM(this, o)).ToList() })
			.Where(x => x.Offers.Any()));
		SelectedPosition = Positions[id];
		OtherStats = new(upd.Hero.Stats.Where(s => s.StatId.StartsWith(Hero.HeroId))
			.Select(s => new HeroStatVM { Stat = s, Name = s.StatId[Hero.HeroId.Length..]}));
		AbilityText = upd.AbilityText;
	}

	internal class HeroStatVM
	{
		public StatData Stat { get; set; } = null!;
		public string Name { get; set; } = null!;
	}

	internal class PositionVM
	{
		public StatData Stat { get; set; } = null!;
		public string Name { get; set; } = null!;
		public List<OfferVM> Offers { get; set; } = null!;
		public Visibility ConvertedVis => Stat.RawValue == Stat.Value ? Visibility.Collapsed : Visibility.Visible;

		public string DescCode => "str:Stat:Test:Desc";
	}

	internal class OfferVM
	{
		private ShopPageVM _ctx;
		public OfferVM(ShopPageVM shopPage, OfferData offer) { _ctx = shopPage; Offer = offer; }

		public OfferData Offer { get; }
		public string OfferText => $"{Offer.StatAmount.RawValue} for {Offer.Cost}";
		public bool Opt => Offer.IsOpt;
		public bool IsAvailiable => _ctx.Hero?.Money >= Offer.Cost;
	}
}
