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

	private List<int> _framesMove = new() {0};
	private List<int> _framesAttack = new() {1};
	private List<int> _framesDead = new() {2};

	public string HeroName {
		get { return (string) GetValue(HeroNameProperty); }
		set { UpdateHeroName(value); SetValue(HeroNameProperty, value); }
	}
	public static readonly DependencyProperty HeroNameProperty =
	DependencyProperty.Register("HeroName", typeof(string),
		typeof(HeroDisplay), new PropertyMetadata("Auxillary:Default",
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
		SetFrames();
		UpdateFrame();
	}

	private void SetFrames()
	{
		_framesMove = _sprite!.MoveFrames switch {
			5 => new List<int>() { 2, 1, 2, 0, 4, 3, 4, 0 },
			4 => new List<int>() { 1, 0, 2, 3, 2, 0 },
			1 => new List<int>() { 0 },
			_ => throw new NotImplementedException()
		};

		_framesAttack = new();
		for (int f = _sprite.MoveFrames; f < _sprite.MoveFrames + _sprite.AttackFrames; ++f)
			_framesAttack.Add(f);
		_framesAttack.Add(0);

		_framesDead = new();
		for (int f = _sprite.MoveFrames + _sprite.AttackFrames; f < _sprite.TotalFrames; ++f)
			_framesDead.Add(f);
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

	public HeroDisplay Move(int ms, int cycles = 1, Dir? direction = null) => Move(TimeSpan.FromMilliseconds(ms), cycles, direction);
	public HeroDisplay Move(TimeSpan duration, int cycles = 1, Dir? direction = null)
	{
		var d = duration/cycles;
		for (int i = 0; i < cycles; ++i)
			AnimMove(d, direction);
		return this;
	}

	public HeroDisplay Attack(int ms, Dir? direction = null) => Attack(TimeSpan.FromMilliseconds(ms), direction);
	public HeroDisplay Attack(TimeSpan duration, Dir? direction = null) => AnimAttack(duration, direction);

	public HeroDisplay Stop(int ms, Dir? direction = null) => Stop(TimeSpan.FromMilliseconds(ms), direction);
	public HeroDisplay Stop(TimeSpan duration, Dir? direction = null) => AnimStop(duration, direction);

	public HeroDisplay Die(int ms, Dir? direction = null) => Die(TimeSpan.FromMilliseconds(ms), direction);
	public HeroDisplay Die(TimeSpan duration, Dir? direction = null) => AnimDie(duration, direction);


	public void StartAnim()
	{
		_cFrames.Duration = new(_time);
		_dFrames.Duration = new(_time);
		BeginAnimation(FrameProperty, _cFrames, HandoffBehavior.SnapshotAndReplace);
		BeginAnimation(DirectionProperty, _dFrames, HandoffBehavior.SnapshotAndReplace);
		_cFrames = new();
		_dFrames = new();
		_time = TimeSpan.Zero;
	}

	private Int32AnimationUsingKeyFrames _cFrames = new();
	private ObjectAnimationUsingKeyFrames _dFrames = new();
	private TimeSpan _time = TimeSpan.Zero;

	private HeroDisplay AnimMove(TimeSpan duration, Dir? direction = null)
	{
		if (direction is not null) 
			_dFrames.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = _time, Value = direction });
		TimeSpan tx = duration / _framesMove.Count;
		_framesMove.ForEach(f => AddFrame(f, tx));
		return this;
	}

	private void AddFrame(int frame, TimeSpan tx)
	{
		var f = new DiscreteInt32KeyFrame{ KeyTime = _time, Value = frame };
		_cFrames.KeyFrames.Add(f);
		_time += tx;
	}

	private HeroDisplay AnimAttack(TimeSpan duration, Dir? direction = null)
	{
		if (direction is not null)
			_dFrames.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = _time, Value = direction });
		TimeSpan tx = duration / _framesAttack.Count;
		_framesAttack.ForEach(f => AddFrame(f, tx));
		return this;
	}

	private HeroDisplay AnimStop(TimeSpan duration, Dir? direction = null)
	{
		if (direction is not null)
			_dFrames.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = _time, Value = direction });
		AddFrame(0, duration);
		return this;
	}

	private HeroDisplay AnimDie(TimeSpan duration, Dir? direction = null)
	{
		if (direction is not null)
			_dFrames.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = _time, Value = direction });
		TimeSpan tx = duration / _framesDead.Count;
		_framesDead.ForEach(f => AddFrame(f, tx));
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