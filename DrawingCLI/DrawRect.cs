using System;

namespace DrawingCLI
{
	public class DrawRect : IDrawableCLI
	{
		public Rect Rect { get; set; } // rekt

		#region Rect Properties

		public int Column => Rect.Column;
		public int Row => Rect.Row;
		public int Width => Rect.Width;
		public int Height => Rect.Height;

		public int Left => Rect.Column;
		public int Right => Rect.Column + Rect.Width - 1;
		public int Top => Rect.Row;
		public int Bottom => Rect.Row + Rect.Height - 1;

		#endregion Rect Properties

		public Colors Colors { get; set; }

		public void Draw()
		{
			Colors.Set();
			Print.Pos(Column, Row, "█" + "".PadRight(Width - 2, '▀') + "█");
			for (int row = Top + 1; row < Bottom; ++row)
				Print.Pos(Column, row, "█" + "".PadRight(Width - 2) + "█");
			Print.Pos(Column, Bottom, "█" + "".PadRight(Width - 2, '▄') + "█");
			Console.SetCursorPosition(Column + 2, Row + 1);
			Console.ResetColor();
		}

		public static DrawRect Random(Random random)
		{
			int width = random.Next(6, Console.WindowWidth - 3),
			height = random.Next(3, Console.WindowHeight - 1),
			column = random.Next(2, Console.WindowWidth - width - 1),
			row = random.Next(1, Console.WindowHeight - height - 1);

			return new DrawRect {
				Rect = new() {
					Width = width, Height = height, Column = column, Row = row,
				},
				Colors = Colors.Random(random),
			};
		}
	}

	public class DrawTextRect : DrawRect, IDrawableCLI
	{
		private const int PADDING_TOP = 1;
		private const int PADDING_BOT = 1;
		private const int PADDING_LEFT = 2;
		private const int PADDING_RIGHT = 2;

		public string Text { get; set; }
		public ConsoleColor TextColor { get; set; }

		public new void Draw()
		{
			base.Draw();
			Rect printArea = new() {
				Column = Rect.Column + PADDING_LEFT,
				Row = Rect.Row + PADDING_TOP,
				Width = Rect.Width - PADDING_LEFT - PADDING_RIGHT,
				Height = Rect.Height - PADDING_TOP - PADDING_BOT
			};
			Print.Colors(TextColor, Colors.BGColor);
			Print.Text(printArea, Text);
			Console.ResetColor();
		}

		public DrawTextRect(DrawRect drawRect, string text)
		{
			Rect = drawRect.Rect;
			Colors = drawRect.Colors;
			Text = text;
			TextColor = (ConsoleColor) 15 - (int) Colors.BGColor;
		}
	}
}