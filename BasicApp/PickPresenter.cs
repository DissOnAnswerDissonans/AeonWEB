using Aeon.Core;
using DrawingCLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aeon.BasicApp
{
	internal class PickPresenter
	{
		public List<Type> _heroes;
		private readonly Colors ColorSelected = new() { Color = ConsoleColor.Green, BGColor = ConsoleColor.DarkGreen };

		private int _choice = 0;

		private const int ROWS = 5;
		private const int HEIGHT = 3;
		private const int WIDTH = 24;

		public PickPresenter()
		{
			var heroesAssembly = Assembly.Load("Aeon.Heroes");
			_heroes = heroesAssembly.GetTypes()
				.Where(t => t.BaseType == typeof(Hero))
				//.Select(t => (Hero) Activator.CreateInstance(t))
				.ToList();
		}

		public Hero PickHero(int player)
		{
			Console.ResetColor();
			Console.Clear();

			var selectors = new List<DrawTextRect>();
			for (int i = 0; i < _heroes.Count; ++i) {
				var rect = new DrawRect {
					Colors = Program.PlayerColors[player],
					Rect = new Rect {
						Column = 2 + i / ROWS * (WIDTH + 2),
						Row = 1 + (1 + HEIGHT) * (i % ROWS),
						Height = HEIGHT,
						Width = WIDTH
					}
				};
				selectors.Add(new DrawTextRect(rect, Info.AboutHero(_heroes[i]).Name.ToString()));
			}

			_choice = 0;

			do {
				for (int i = 0; i < selectors.Count; ++i) {
					selectors[i].Colors = _choice == i ? ColorSelected : Program.PlayerColors[player];
					selectors[i].Draw();
				}
				Console.SetCursorPosition(0, 0);
			} while (Input(Console.ReadKey()));

			Console.ResetColor();

			return (Hero) Activator.CreateInstance(_heroes[_choice]);
		}

		private bool Input(ConsoleKeyInfo info)
		{
			switch (info.Key) {
			case ConsoleKey.UpArrow:
				_choice -= 1; break;
			case ConsoleKey.DownArrow:
				_choice += 1; break;
			case ConsoleKey.LeftArrow:
				_choice -= ROWS; break;
			case ConsoleKey.RightArrow:
				_choice += ROWS; break;
			case ConsoleKey.Enter:
				return false;
			};
			_choice = Math.Clamp(_choice, 0, _heroes.Count - 1);
			return true;
		}

	}
}
