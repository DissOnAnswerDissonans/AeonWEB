using System;

namespace DrawingCLI
{
	internal struct DrawPoint : IDrawableCLI
	{
		public Point Point { get; init; }
		public char Char { get; init; }
		public Colors Colors { get; init; }

		public static DrawPoint Random(Random random) => new() {
			Point = new Point() {
				Column = random.Next(Console.WindowWidth),
				Row = random.Next(Console.WindowHeight),
			},
			Char = (char) random.Next(0x21, 0x7f),
			Colors = Colors.Random(random),
		};

		public void Draw()
		{
			Print.Pos(Point, Char);
			Console.SetCursorPosition(Point.Column, Point.Row);
		}
	}
}