using System;

namespace DrawingCLI
{
	public struct Point
	{
		public int Column { get; init; }
		public int Row { get; init; }
	}

	public struct Rect
	{
		public int Column { get; init; }
		public int Row { get; init; }
		public int Width { get; init; }
		public int Height { get; init; }
	}

	internal class MutableRect
	{
		public int Column { get; set; }
		public int Row { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		public static implicit operator Rect(MutableRect mutable) => new Rect {
			Column = mutable.Column, Row = mutable.Row,
			Height = mutable.Height, Width = mutable.Width
		};
	}

	public struct Colors
	{
		public ConsoleColor Color { get; init; }
		public ConsoleColor BGColor { get; init; }

		public void Set() => Print.Colors(Color, BGColor);

		public static Colors Random(Random random) => new() {
			Color = (ConsoleColor) random.Next(16),
			BGColor = (ConsoleColor) random.Next(16),
		};
	}

}
