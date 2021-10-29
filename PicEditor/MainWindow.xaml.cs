using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PicEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private int LeftColor {
			get => __leftColor;
			set {
				__leftColor = value;
				_palLeft.Fill = _brushes[__leftColor];
			}
		}
		private int RightColor {
			get => __rightColor;
			set {
				__rightColor = value;
				_palRight.Fill = _brushes[__rightColor];
			}
		}

		private int __leftColor;
		private int __rightColor;

		private byte[] _bytes;

		private int _mouseButton;
		private readonly Brush black = new SolidColorBrush(Color.FromRgb(0, 0, 0));

		public MainWindow()
		{
			InitializeComponent();
			for (int i = 0; i < 16; ++i) {
				var rect = new Rectangle
				{ Fill = _brushes[i], Stroke = black, Margin = new Thickness(2)};
				int c = i;
				rect.MouseLeftButtonDown += (obj, args) => LeftColor = c;
				rect.MouseRightButtonDown += (obj, args) => RightColor = c;
				_palette.Children.Add(rect);
			}

			LeftColor = 15;
			RightColor = 0;

			window.MouseLeftButtonDown += (obj, args) => _mouseButton = 1;
			window.MouseRightButtonDown += (obj, args) => _mouseButton = 2;
			window.MouseUp += (obj, args) => _mouseButton = 0;

			Button_Click(this, null);
		}

		private void ResizeBase() => _base.Width = _base.ActualHeight * _base.Columns / _base.Rows;

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			int x, y;
			try {
				x = Convert.ToInt32(_sizeX.Text);
				y = Convert.ToInt32(_sizeY.Text);
			}
			catch (FormatException) {
				_text.Text = "В полях X, Y какая-то херня";
				return;
			}

			if (x < 1 || x > 64 || y < 1 || y > 64) {
				_text.Text = "X, Y должны быть в пределах 1 — 64";
				return;
			}

			if (y % 2 != 0) {
				_text.Text = "Y должен быть чётным";
				return;
			}

			_base.Rows = y;
			_base.Columns = x;

			_bytes = new byte[x * y / 2];

			_base.Children.Clear();
			for (int i = 0; i < x * y; ++i) {
				int tx = i;
				var kok = new Rectangle { Stroke = black, Fill = _brushes[0] };

				kok.MouseMove += PaintCell;
				_base.Children.Add(kok);

				void PaintCell(object sender, MouseEventArgs e)
				{
					int color = _mouseButton switch{
						1 => LeftColor,
						2 => RightColor,
						_ => -1
					};
					if (color == -1) return;

					(sender as Rectangle).Fill = _brushes[color];
					int r = tx % x, z = tx / x, pos = r + x*(z/2);
					_bytes[pos] = (byte) (z % 2 == 0
						? (_bytes[pos] & 0xF0 | color)
						: (_bytes[pos] & 0x0F | 16 * color));
					UpdateText();
				}
			}

			ResizeBase();

		}

		private void UpdateText()
		{
			StringBuilder @string = new();
			foreach (byte b in _bytes)
				@string.Append($"{b}, ");
			_text.Text = @string.ToString();
		}

		private void window_SizeChanged(object sender, SizeChangedEventArgs e) => ResizeBase();

		private readonly Brush[] _brushes = new Brush[]{
			new SolidColorBrush(Color.FromRgb(012, 012, 012)), // 0
			new SolidColorBrush(Color.FromRgb(000, 055, 218)), // 1
			new SolidColorBrush(Color.FromRgb(019, 161, 014)), // 2
			new SolidColorBrush(Color.FromRgb(058, 150, 221)), // 3
			new SolidColorBrush(Color.FromRgb(197, 015, 031)), // 4
			new SolidColorBrush(Color.FromRgb(136, 023, 152)), // 5
			new SolidColorBrush(Color.FromRgb(193, 156, 000)), // 6
			new SolidColorBrush(Color.FromRgb(204, 204, 204)), // 7
			new SolidColorBrush(Color.FromRgb(118, 118, 118)), // 8
			new SolidColorBrush(Color.FromRgb(059, 120, 255)), // 9
			new SolidColorBrush(Color.FromRgb(022, 198, 012)), // A
			new SolidColorBrush(Color.FromRgb(097, 214, 214)), // B
			new SolidColorBrush(Color.FromRgb(231, 072, 086)), // C
			new SolidColorBrush(Color.FromRgb(180, 000, 158)), // D
			new SolidColorBrush(Color.FromRgb(249, 241, 165)), // E
			new SolidColorBrush(Color.FromRgb(242, 242, 242)), // F
		};
	}
}