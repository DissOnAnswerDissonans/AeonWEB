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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Aeon.Base.BattleTurn;

namespace Aeon.WindowsClient.Views;
/// <summary>
/// Interaction logic for RoundTimer.xaml
/// </summary>
public partial class RoundTimer : UserControl
{
	public RoundTimer()
	{
		InitializeComponent();
	}

	public double RemainingTime {
		get { return (double) GetValue(RemainingTimeProperty); }
		set { SetValue(RemainingTimeProperty, value); }
	}
	private static readonly DependencyProperty RemainingTimeProperty =
	DependencyProperty.Register("RemainingTime", typeof(double), typeof(RoundTimer), new PropertyMetadata(0.0,
		(c, x) => ((RoundTimer)c).Tock((double) x.NewValue)));

	public double Time {
		get { return (double) GetValue(TimeProperty); }
		set { SetValue(TimeProperty, value); }
	}
	public static readonly DependencyProperty TimeProperty =
	DependencyProperty.Register("Time", typeof(double), typeof(RoundTimer), new PropertyMetadata(0.0,
		(c, x) => ((RoundTimer)c).SetTime((double) x.NewValue)));

	public void SetTime(double sec) => SetTime(TimeSpan.FromSeconds(sec));
	public void SetTime(TimeSpan time)
	{
		var anim = new DoubleAnimation(time.TotalSeconds, 0.0, new(time), FillBehavior.HoldEnd);
		BeginAnimation(RemainingTimeProperty, anim);
	}

	private void Tock(double value)
	{
		val.Text = Math.Ceiling(value).ToString();
		SetPath(Time <= 0 ? 0 : value / Time);
	}

	private void SetPath(double fill)
	{
		var w = ActualWidth / 2;
		var h = ActualHeight / 2;

		Geometry G1 = new EllipseGeometry(new Point(w, h), w-1, h-1);
		Geometry G2 = new CombinedGeometry(
			new EllipseGeometry(new Point(w, h), w-7, h-7),
			new PathGeometry(new List<PathFigure>(){ new PathFigure(new(w, h), GetFigs(ActualWidth, ActualHeight, fill), closed: true) }));
		path.Data = fill <= 0 ? new() : new CombinedGeometry(GeometryCombineMode.Exclude, G1, G2);
	}

	private IEnumerable<PathSegment> GetFigs(double width, double height, double fill)
	{
		if (fill >= 1) yield break;
		if (fill <= 0) yield break;
		fill = 1 - fill;

		(double x, double y) tx = Math.SinCos(fill * 2 * Math.PI);
		tx = ((tx.x + 1) / 2, (-tx.y + 1) / 2);
		Point kxt = new(tx.x * width, tx.y * height);

		yield return new LineSegment(new(width / 2, 0), false);
		yield return new ArcSegment(kxt, new(width/2, height/2), 0, true, SweepDirection.Clockwise, true);
	}

	// Figures="M 64 64 L 64 0 L 128 0 L 128 128 L 0 128 L 0 0 L 64 0"
}
