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
using System.Windows.Media.Animation;

namespace Aeon.WindowsClient.Views;
/// <summary>
/// Interaction logic for HeroDisplay.xaml
/// </summary>
public partial class HeroDisplay : UserControl
{
	private SpriteInfo? _sprite;
	private Size? _spriteSize;

	private Int32KeyFrameCollection _framesMove;
	private Int32KeyFrameCollection _framesAttack;
	private Int32KeyFrameCollection _framesDead;

	public string HeroName {
		get { return (string) GetValue(HeroNameProperty); }
		set { UpdateHeroName(value); SetValue(HeroNameProperty, value); }
	}
	public static readonly DependencyProperty HeroNameProperty =
	DependencyProperty.Register("HeroName", typeof(string),
		typeof(HeroDisplay), new PropertyMetadata("Aeon.Heroes:Banker",
			(d, args) => ((HeroDisplay)d).UpdateHeroName((string)args.NewValue)));

	public Dir Direction {
		get { return (Dir) GetValue(DirectionProperty); }
		set { SetValue(DirectionProperty, value); }
	}
	public static readonly DependencyProperty DirectionProperty = 
		DependencyProperty.Register("Direction", typeof(Dir), typeof(HeroDisplay), 
			new PropertyMetadata(Dir.N, (d, args) => ((HeroDisplay)d).UpdateFrame()));

	public int Frame {
		get { return (int) GetValue(FrameProperty); }
		set { SetValue(FrameProperty, value); }
	}
	public static readonly DependencyProperty FrameProperty = 
		DependencyProperty.Register("Frame", typeof(int), typeof(HeroDisplay), 
			new PropertyMetadata(0, (d, args) => ((HeroDisplay)d).UpdateFrame()));

	public HeroDisplay() => InitializeComponent();

	public void UpdateHeroName(string name)
	{
		_sprite = SpriteProvider.HeroName(name);
		img.RenderTransform = new ScaleTransform(5, _sprite.TotalFrames);
		img.Source = _sprite.Source;
		_spriteSize = new(img.Source.Width / 5, img.Source.Height / _sprite.TotalFrames);
		CompileAnimations();
		UpdateFrame();
	}

	private void CompileAnimations()
	{
		_framesMove = new Int32KeyFrameCollection();
		(_sprite!.MoveFrames switch {
			5 => new List<int>() { 0, 2, 1, 2, 0, 4, 3, 4 },
			4 => new List<int>() { 0, 1, 0, 2, 3, 2 },
			1 => new List<int>() { 0 },
			_ => throw new NotImplementedException()
		}).ForEach(f => _framesMove.Add(new DiscreteInt32KeyFrame(f)));

		_framesAttack = new Int32KeyFrameCollection();
		for (int f = _sprite.MoveFrames; f < _sprite.MoveFrames + _sprite.AttackFrames; ++f)
			_framesAttack.Add(new DiscreteInt32KeyFrame(f));
		_framesAttack.Add(new DiscreteInt32KeyFrame(0));

		_framesDead = new Int32KeyFrameCollection();
		for (int f = _sprite.MoveFrames + _sprite.AttackFrames; f < _sprite.TotalFrames; ++f)
			_framesDead.Add(new DiscreteInt32KeyFrame(f));
	}

	private void UpdateFrame()
	{
		int takeDir = (int) Direction;
		int frames = _sprite?.TotalFrames ?? 10;
		if (takeDir < 5) {
			img.RenderTransformOrigin = 
				new Point(takeDir / 4.0, (double) Frame / (frames - 1));
			img.LayoutTransform = Transform.Identity;
		} else {
			img.RenderTransformOrigin = 
				new Point((takeDir - 4) / 4.0, (double) Frame / (frames - 1));
			img.LayoutTransform = new ScaleTransform(-1, 1);
		}
	}

	public HeroDisplay Move(int ms) => Move(TimeSpan.FromMilliseconds(ms));
	public HeroDisplay Move(TimeSpan duration) => AnimMove(duration, HandoffBehavior.SnapshotAndReplace);
	public HeroDisplay ThenMove(int ms) => ThenMove(TimeSpan.FromMilliseconds(ms));
	public HeroDisplay ThenMove(TimeSpan duration) => AnimMove(duration, HandoffBehavior.Compose);

	public HeroDisplay Attack(int ms) => Attack(TimeSpan.FromMilliseconds(ms));
	public HeroDisplay Attack(TimeSpan duration) => AnimAttack(duration, HandoffBehavior.SnapshotAndReplace);
	public HeroDisplay ThenAttack(int ms) => ThenAttack(TimeSpan.FromMilliseconds(ms));
	public HeroDisplay ThenAttack(TimeSpan duration) => AnimAttack(duration, HandoffBehavior.Compose);

	public HeroDisplay Stop(int ms) => Stop(TimeSpan.FromMilliseconds(ms));
	public HeroDisplay Stop(TimeSpan duration) => AnimStop(duration, HandoffBehavior.SnapshotAndReplace);
	public HeroDisplay ThenStop(int ms) => ThenStop(TimeSpan.FromMilliseconds(ms));
	public HeroDisplay ThenStop(TimeSpan duration) => AnimStop(duration, HandoffBehavior.Compose);

	public HeroDisplay Die(int ms) => Die(TimeSpan.FromMilliseconds(ms));
	public HeroDisplay Die(TimeSpan duration) => AnimDie(duration, HandoffBehavior.SnapshotAndReplace);
	public HeroDisplay ThenDie(int ms) => ThenDie(TimeSpan.FromMilliseconds(ms));
	public HeroDisplay ThenDie(TimeSpan duration) => AnimDie(duration, HandoffBehavior.Compose);


	private HeroDisplay AnimMove(TimeSpan duration, HandoffBehavior behavior)
	{
		Int32AnimationUsingKeyFrames a = new();
		a.KeyFrames = _framesMove;
		a.RepeatBehavior = new RepeatBehavior(duration);
		BeginAnimation(FrameProperty, a, behavior);
		return this;
	}

	private HeroDisplay AnimAttack(TimeSpan duration, HandoffBehavior behavior)
	{
		Int32AnimationUsingKeyFrames a = new();
		a.KeyFrames = _framesAttack;
		a.Duration = new Duration(duration);
		BeginAnimation(FrameProperty, a, behavior);
		return this;
	}

	private HeroDisplay AnimStop(TimeSpan duration, HandoffBehavior behavior)
	{
		Int32AnimationUsingKeyFrames a = new();
		var st = new Int32KeyFrameCollection();
		st.Add(new DiscreteInt32KeyFrame(0));
		a.KeyFrames = st;
		a.Duration = new(duration);
		BeginAnimation(FrameProperty, a, behavior);
		return this;
	}

	private HeroDisplay AnimDie(TimeSpan duration, HandoffBehavior behavior)
	{
		Int32AnimationUsingKeyFrames a = new();
		a.KeyFrames = _framesDead;
		a.Duration = new Duration(duration);
		BeginAnimation(FrameProperty, a, behavior);
		return this;
	}

	public enum Dir { N, NE, E, SE, S, SW, W, NW }
}

public static class SpriteProvider
{
	public static SpriteInfo HeroName(string name)
	{
		var sprite = (SpriteInfo) App.Current.Resources[$"Sprite:{name}"];
		sprite.HeroName = name;
		return sprite;
	}
}