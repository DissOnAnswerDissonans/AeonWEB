using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace Aeon.WindowsClient;

public class SpriteInfo : DependencyObject
{
	public string HeroName {
		get { return (string) GetValue(HeroNameProperty); }
		set { SetValue(HeroNameProperty, value); }
	}
	public static readonly DependencyProperty HeroNameProperty =
	DependencyProperty.Register("HeroName", typeof(string), typeof(SpriteInfo), new PropertyMetadata(""));

	public ImageSource Source {
		get { return (ImageSource) GetValue(SourceProperty); }
		set { SetValue(SourceProperty, value); }
	}
	public static readonly DependencyProperty SourceProperty =
	DependencyProperty.Register("Source", typeof(ImageSource), typeof(SpriteInfo));

	public int MoveFrames {
		get { return (int) GetValue(MoveFramesProperty); }
		set { SetValue(MoveFramesProperty, value); }
	}
	public static readonly DependencyProperty MoveFramesProperty =
	DependencyProperty.Register("MoveFrames", typeof(int), typeof(SpriteInfo), new PropertyMetadata(0));

	public int AttackFrames {
		get { return (int) GetValue(AttackFramesProperty); }
		set { SetValue(AttackFramesProperty, value); }
	}
	public static readonly DependencyProperty AttackFramesProperty =
	DependencyProperty.Register("AttackFrames", typeof(int), typeof(SpriteInfo), new PropertyMetadata(0));

	public int DeadFrames {
		get { return (int) GetValue(DeadFramesProperty); }
		set { SetValue(DeadFramesProperty, value); }
	}
	public static readonly DependencyProperty DeadFramesProperty =
	DependencyProperty.Register("DeadFrames", typeof(int), typeof(SpriteInfo), new PropertyMetadata(0));

	public int TotalFrames => MoveFrames + AttackFrames + DeadFrames;

	public Range MoveRange => new(0, MoveFrames);
	public Range AttackRange => new(MoveFrames, MoveFrames + AttackFrames);
	public Range DeadRange => new(MoveFrames + AttackFrames, TotalFrames);
}
