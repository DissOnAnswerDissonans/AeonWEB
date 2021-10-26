using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingCLI
{
	public class DrawLogZone : IDrawableCLI
	{
		public Rect Rect { get; init; }
		
		public bool BottomUp { get; init; }
		public bool LeftAligned { get; init; }

		private List<string> _logs = new();
		private List<Colors> _colors = new();

		public void Add(string str, ConsoleColor color = ConsoleColor.White, ConsoleColor colorBG = ConsoleColor.Black)
		{
			_logs.Add(str);
			_colors.Add(new Colors { Color = color, BGColor = colorBG });
		}

		public void Draw()
		{
			int entriesToDisp = Math.Min(_logs.Count, Rect.Height);
			int firstRow = BottomUp? Rect.Row + Rect.Height - entriesToDisp : Rect.Row;
			for (int row = firstRow, i = entriesToDisp; i > 0; --i, ++row) {
				_colors[^i].Set();
				string s = LeftAligned? _logs[^i].PadRight(Rect.Width) : _logs[^i].PadLeft(Rect.Width);
				Print.Pos(Rect.Column, row, s);
			}
			Console.ResetColor();
		}
	}
}
