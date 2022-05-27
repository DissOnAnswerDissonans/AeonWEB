using Aeon.Base;
using Aeon.WindowsClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Aeon.WindowsClient.ViewModels;

internal class HeroSelectVM : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public ObservableCollection<HeroPickerVM> Heroes { get; set; } = new();
	public ObservableCollection<PlayerPicksVM> Players { get; set; } = new();

	public bool PickAvailiable { get; set; }
	public bool IsPicked { get; set; } = false;

	internal class HeroPickerVM
	{
		public string Name { get; set; } = "??";
		public int ID { get; set; }
		public string Assembly { get; set; } = "??";

		public TrofCommand Pick { get; set; }

		public HeroPickerVM(HeroInfo hero, HeroSelectVM vm)
		{
			Name = hero.Name;
			ID = hero.ID;
			Assembly = hero.AssemblyName;
			Pick = new(() => {
				vm.IsPicked = true;
				vm.PickAvailiable = false;
				App.Game.SelectHero.Send(ID);
			});
		}
	}

	internal class PlayerPicksVM
	{
		public string PlayerName { get; }
		public string Hero { get; }

		public PlayerPicksVM(HeroSelection s, bool show)
		{
			PlayerName = s.Nickname;
			Hero = s.Hero switch {
				null => "…",
				_ => show ? s.Hero.Name : "???"
			};
		}
	}
}
