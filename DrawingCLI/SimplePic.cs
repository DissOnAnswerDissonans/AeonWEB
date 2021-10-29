using System;
using System.Collections;

namespace DrawingCLI
{
	public class SimplePic
	{
		private readonly BitArray _pixels;
		public int Width { get; init; }
		public int Height { get; init; }

		public SimplePic(int width, int height, BitArray array)
		{
			Width = width;
			Height = height;
			_pixels = array;
		}

		public void DrawIn(int column, int row, Colors colors)
		{
			colors.Set();
			for (int y = 0; y < Height; y += 2) {
				for (int x = 0; x < Width; ++x) {
					(bool, bool) p = (_pixels[y*Width+x], _pixels[(y+1)*Width+x]);
					Print.Pos(column + x, row + y / 2, p switch {
						(false, false) => ' ',
						(false, true) => '▄',
						(true, false) => '▀',
						(true, true) => '█'
					});
				}
			}
			Console.ResetColor();
		}
	}
}
