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
/// Interaction logic for RoomList.xaml
/// </summary>
public partial class RoomList : Page
{
	public RoomList()
	{
		InitializeComponent();
	}

	private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (e.AddedItems.Count > 0)
			(DataContext as ViewModels.RoomsVM)?.Join.Execute(e.AddedItems[0]);
	}
}
