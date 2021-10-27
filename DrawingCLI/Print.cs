using System;

namespace DrawingCLI
{
    public static class Print
    {
        public static void Colors(ConsoleColor fore, ConsoleColor back)
        {
            Console.ForegroundColor = fore;
            Console.BackgroundColor = back;
        }

        public static void Pos(int column, int row, object obj)
        {
            Console.SetCursorPosition(column, row);
            Console.Write(obj);
        }

        public static void Pos(Point point, object obj) => Pos(point.Column, point.Row, obj);

        public static void Text(Rect rect, string text)
        {
            string[] words = text.Split(' ');
            string[] lines = new string[rect.Height];
            for (int i = 0; i < rect.Height; ++i)
                lines[i] = "";

            int row = 0;

            foreach (string word in words) {
                if (word.Length > rect.Width - lines[row].Length) {
                    lines[row] = lines[row].Trim();
                    if (++row >= rect.Height) break;
                }
                lines[row] += (word + " ");
            }

            for (int l = 0; l < rect.Height; ++l) {
                Pos(rect.Column, rect.Row + l, lines[l]);
            }
        }

        public static void Clear()
        {
            Console.ResetColor();
            Console.Clear();
        }

		internal static void Colors(Colors color) => Colors(color.Color, color.BGColor);
	}
}
