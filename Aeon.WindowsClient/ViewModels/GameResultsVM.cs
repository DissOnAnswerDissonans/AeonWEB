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
using System.Windows.Input;
using System.Windows.Media;

namespace Aeon.WindowsClient.ViewModels;

internal class GameResultsVM : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public FinalResult? Result { get; set; }

	public RoundScoreSummary? Scores => Result?.Scores;
	public RoundScoreSummary.Entry? Winner => Scores?.Entries[0];
	public RoundScoreSummary.Entry? Loser => Scores?.Entries[1];
	public ObservableCollection<RoundScoreSummary.Entry>? Losers => Scores is null ? null : new(Scores?.Entries.Where(s => s != Winner)!);
}
