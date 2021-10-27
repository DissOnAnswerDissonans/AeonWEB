using System;

namespace DrawingCLI
{
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
}
