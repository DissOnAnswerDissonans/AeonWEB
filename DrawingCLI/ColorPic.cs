using System;

namespace DrawingCLI
{
	public class ColorPic
	{
		private readonly byte[] _pixels;
		public int Width { get; init; }
		public int Height { get; init; }

		public ColorPic(int width, int height, byte[] array)
		{
			Width = width;
			Height = height;
			_pixels = array;
		}

		public void DrawIn(int column, int row)
		{
			for (int y = 0; y < Height; ++y) {
				for (int x = 0; x < Width; ++x) {
					Print.Colors(
						(ConsoleColor) (_pixels[y * Width + x] % 16),
						(ConsoleColor) (_pixels[y * Width + x] / 16)
					);
					Print.Pos(column + x, row + y, '▀');
				}
			}
		}
	}
}
