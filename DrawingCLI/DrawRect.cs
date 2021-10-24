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

    class DrawRect : IDrawableCLI
    {
        public Rect Rect { get; init; } // rekt
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
        public Colors Colors { get; init; }

        public void Draw()
        {
            Colors.Set();
            Print.Pos(Column, Row, "█" + "".PadRight(Width - 2, '▀') + "█");
            for (int row = Top + 1; row < Bottom; ++row)
                Print.Pos(Column, row, "█" + "".PadRight(Width - 2) + "█");
            Print.Pos(Column, Bottom, "█" + "".PadRight(Width - 2, '▄') + "█");
            Console.SetCursorPosition(Column + 2, Row + 1);
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

    class DrawTextRect : DrawRect, IDrawableCLI
    {
        const int PADDING_TOP = 1;
        const int PADDING_BOT = 1;
        const int PADDING_LEFT = 2;
        const int PADDING_RIGHT = 2;

        public string Text { get; init; }
        public ConsoleColor TextColor { get; init; }

        public new void Draw()
        {
            base.Draw();
            Rect printArea = new() {
                Column = Rect.Column + PADDING_LEFT,
                Row = Rect.Row + PADDING_TOP,
                Width = Rect.Width - PADDING_LEFT - PADDING_RIGHT,
                Height = Rect.Height - PADDING_TOP - PADDING_BOT
            };
            Console.ForegroundColor = TextColor;
            Print.Text(printArea, Text);
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
