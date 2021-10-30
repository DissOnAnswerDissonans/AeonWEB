using System;
using System.Collections.Generic;

namespace DrawingCLI
{
	public class Table<T> : IDrawableCLI
	{
		private readonly MutableRect _rect = new() { Width = 1, Height = 1 };
		public Rect Rect => _rect;

		public int Rows { get; private set; } = 0;
		public int Columns { get; private set; } = 0;

		public List<int> ColumnPos { get; } = new() { 1 };
		public List<int> RowPos { get; } = new() { 1 };

		private readonly Dictionary<(int, int), T> _values = new();

		private readonly Dictionary<(int, int), string> _names = new();


		private Colors _color = new() {Color = ConsoleColor.Gray, BGColor = ConsoleColor.Black};

		private Colors _textColor = new() {Color = ConsoleColor.Gray, BGColor = ConsoleColor.Black};

		private readonly Dictionary<(int, int), Colors> _colors = new();

		public void SetColor(ConsoleColor color, ConsoleColor colorBG = ConsoleColor.Black) => _color = new Colors { Color = color, BGColor = colorBG };
		public void SetColor(Colors colors) => _color = colors;

		public void SetTextColor(ConsoleColor color, ConsoleColor colorBG = ConsoleColor.Black) => _textColor = new Colors { Color = color, BGColor = colorBG };
		public void SetTextColor(Colors colors) => _textColor = colors;

		public void SetTextColor(int row, int column,
			ConsoleColor color, ConsoleColor colorBG = ConsoleColor.Black) => _colors[(row, column)] = new Colors { Color = color, BGColor = colorBG };

		public void SetTextColorIf(Func<T, bool> func, ConsoleColor color, ConsoleColor colorBG = ConsoleColor.Black)
		{
			for (int row = 0; row < Rows; ++row) {
				for (int column = 0; column < Columns; ++column) {
					if (func(this[row, column]))
						SetTextColor(row, column, color, colorBG);
				}
			}
		}

		public void ResetColor(int row, int column) => _colors.Remove((row, column));

		public Colors GetColor(int row, int column) =>
			_colors.TryGetValue((row, column), out Colors color) ? color : _textColor;

		public T this[int row, int column] {
			get => _values.TryGetValue((row, column), out T value) ? value : default;
			set => _values[(row, column)] = value;
		}

		public T this[int row, int column, string name] {
			set {
				this[row, column] = value;
				_names[(row, column)] = name;
			}
		}

		public Rect AddColumns(params int[] widths)
		{
			foreach (int w in widths) {
				++Columns;
				_rect.Width += w + 1;
				ColumnPos.Add(_rect.Width);
			}
			return Rect;
		}

		public Rect AddRows(int number)
		{
			for (int i = 0; i < number; ++i) {
				++Rows;
				_rect.Height += 2;
				RowPos.Add(_rect.Height);
			}
			return Rect;
		}

		public Rect SetPos(int x, int y)
		{
			_rect.Row = y;
			_rect.Column = x;
			return Rect;
		}

		public void Draw()
		{
			var columns = new ColumnDrawType[_rect.Width];
			var rows = new RowDrawType[_rect.Height];
			{
				foreach (int w in ColumnPos) {
					columns[w - 1] = ColumnDrawType.Middle;
				}
				columns[0] = ColumnDrawType.Left;
				columns[^1] = ColumnDrawType.Right;

				foreach (int w in RowPos) {
					rows[w - 1] = RowDrawType.Middle;
				}
				rows[0] = RowDrawType.Top;
				rows[^1] = RowDrawType.Bottom;
			}
			Print.Colors(_color);

			for (int y = 0; y < _rect.Height; ++y) {
				for (int x = 0; x < _rect.Width; ++x) {
					Print.Pos(x + _rect.Column, y + _rect.Row, DChar(rows[y], columns[x]));
				}
			}

			DrawTextOnly();
		}

		public void DrawTextOnly()
		{
			for (int row = 0; row < Rows; ++row) {
				for (int column = 0; column < Columns; ++column) {

					if (!_names.TryGetValue((row, column), out string s))
						s = this[row, column].ToString();

					Print.Colors(GetColor(row, column));
					Print.Pos(ColumnPos[column] + _rect.Column,
						RowPos[row] + _rect.Row, $" {s} ");
				}
			}
			Console.ResetColor();
		}

		private enum RowDrawType { None = 0, Middle = 1, Top = 2, Bottom = 3 }

		private enum ColumnDrawType { None = 0, Middle = 1, Left = 2, Right = 3 }

		private char DChar(RowDrawType row, ColumnDrawType column) =>
			_bchars[(int) row, (int) column];

		private readonly char[,] _bchars = {
			{' ', '│', '║', '║'},
			{'─', '┼', '╟', '╢'},
			{'═', '╤', '╔', '╗'},
			{'═', '╧', '╚', '╝'},
		};


	}
}
