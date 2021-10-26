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

	class MutableRect
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
        #endregion
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

        public static DrawRect Random(Random random) {
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

	public class ProgressBar : DrawRect
	{
		private int _value;
		public int Value { get => _value; set => _value = Math.Clamp(value, 0, MaxValue); }
		public int MaxValue { get; set; } = 100;

		public ConsoleColor FillColor { get; set; }
		private Colors FillColors => new() { Color = Colors.Color, BGColor = FillColor };

		public void SetValues(int value, int max)
		{
			MaxValue = max;
			Value = value;
		}

		public new void Draw()
		{
			int widthA = (Width - 2)*Value/MaxValue;
			int widthB = (Width - 2) - widthA;

			FillColors.Set();
			Print.Pos(Column, Row, "█" + "".PadRight(widthA, '▀'));
			for (int row = Top + 1; row < Bottom; ++row)
				Print.Pos(Column, row, "█" + "".PadRight(widthA));
			Print.Pos(Column, Bottom, "█" + "".PadRight(widthA, '▄'));

			Colors.Set();
			Print.Pos(Column + widthA + 1, Row, "".PadRight(widthB, '▀') + "█");
			for (int row = Top + 1; row < Bottom; ++row)
				Print.Pos(Column + widthA + 1, row, "".PadRight(widthB) + "█");
			Print.Pos(Column + widthA + 1, Bottom, "".PadRight(widthB, '▄') + "█");

			Console.SetCursorPosition(Column + 2, Row + 1);
			string s = $"{Value}/{MaxValue}";
			Console.Write(s);

			Console.ResetColor();
		}
	}

	public class DrawTextRect : DrawRect, IDrawableCLI
    {
        const int PADDING_TOP = 1;
        const int PADDING_BOT = 1;
        const int PADDING_LEFT = 2;
        const int PADDING_RIGHT = 2;

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
            this.Rect = drawRect.Rect;
            this.Colors = drawRect.Colors;
            this.Text = text;
            this.TextColor = (ConsoleColor) 15 - (int) Colors.BGColor;
        }
    }
}
