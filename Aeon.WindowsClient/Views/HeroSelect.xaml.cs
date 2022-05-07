﻿using Aeon.WindowsClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using static Aeon.WindowsClient.ViewModels.HeroSelectVM;

namespace Aeon.WindowsClient.Views;
/// <summary>
/// Interaction logic for HeroSelect.xaml
/// </summary>
public partial class HeroSelect : Page
{
	HeroSelectVM VM { get; }
	public HeroSelect()
	{
		InitializeComponent();
		VM = (HeroSelectVM) DataContext;

		App.Game.PickPhaseStarted.Subscribe(o => {
			VM.Heroes = new(o.Select(o => new HeroPickerVM(o, VM)));
			VM.PickAvailiable = true;
		});

		App.Game.HeroSelectedAnyone.Subscribe(o => {
			VM.Players = new(o.Select(o => new PlayerPicksVM(o, VM.IsPicked)));
		});
	}
}
