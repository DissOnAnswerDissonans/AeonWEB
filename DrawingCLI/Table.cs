using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingCLI
{
	public class Table : IDrawableCLI
	{
		private MutableRect _rect = new() { Width = 1, Height = 1 };
		public Rect Rect => _rect;

		public int Rows { get; private set; } = 0;
		public int Columns { get; private set; } = 0;

		public List<int> ColumnPos { get; } = new() { 1 };
		public List<int> RowPos { get; } = new() { 1 };

		private Dictionary<(int, int), string> _values = new();

		public string this[int row, int column] {
			get => _values.TryGetValue((row, column), out var value) ? value : "";
			set => _values[(row, column)] = value;
		}

		public Rect AddColumns(params int[] widths)
		{
			foreach (var w in widths) {
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
			ColumnDrawType[] columns = new ColumnDrawType[_rect.Width];
			RowDrawType[] rows = new RowDrawType[_rect.Height];

			{
				foreach (var w in ColumnPos) {
					columns[w-1] = ColumnDrawType.Middle;
				}
				columns[0] = ColumnDrawType.Left;
				columns[^1] = ColumnDrawType.Right;

				foreach (var w in RowPos) {
					rows[w-1] = RowDrawType.Middle;
				}
				rows[0] = RowDrawType.Top;
				rows[^1] = RowDrawType.Bottom;
			}
			
			for (int y = 0; y < _rect.Height; ++y) {
				for (int x = 0; x < _rect.Width; ++x) {
					Print.Pos(x + _rect.Column, y + _rect.Row, DChar(rows[y], columns[x]));
				}
			}

			for (int row = 0; row < Rows; ++row) {
				for (int column = 0; column < Columns; ++column) {
					Print.Pos(ColumnPos[column] + _rect.Column, 
						RowPos[row] + _rect.Row, this[row, column]);
				}
			}

			//string z0 = "║      │   │     │    ║";
			//string z1 = "╟──────┼───┼─────┼────╢";
			//string z2 = "╔══════╤═══╤═════╤════╗";
			//string z3 = "╚══════╧═══╧═════╧════╝";
		}

		enum RowDrawType { None = 0, Middle = 1, Top = 2, Bottom = 3 }
		enum ColumnDrawType { None = 0, Middle = 1, Left = 2, Right = 3 }

		char DChar(RowDrawType row, ColumnDrawType column) =>
			_bchars[(int) row, (int) column];

		readonly char[,] _bchars = { 
			{' ', '│', '║', '║'}, 
			{'─', '┼', '╟', '╢'}, 
			{'═', '╤', '╔', '╗'}, 
			{'═', '╧', '╚', '╝'}, 
		};
	}
}
