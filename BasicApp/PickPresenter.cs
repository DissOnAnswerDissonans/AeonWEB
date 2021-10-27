using System;
using System.Reflection;
using System.Collections.Generic;
using Aeon.Core;
using DrawingCLI;
using System.Linq;

namespace Aeon.BasicApp
{
	class PickPresenter
	{
		public List<Type> _heroes;

		readonly Colors ColorSelected = new() { Color = ConsoleColor.Green, BGColor = ConsoleColor.DarkGreen };

		private int _choice = 0;

		public PickPresenter()
		{
			Assembly heroesAssembly = Assembly.Load("Aeon.Heroes");
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
					Rect = new Rect { Column = 5, Row = 1 + 4 * i, Height = 3, Width = 30 }
				};
				selectors.Add(new DrawTextRect(rect, _heroes[i].ToString()));
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
			case ConsoleKey.Enter:
				return false;
			};
			_choice = Math.Clamp(_choice, 0, _heroes.Count-1);
			return true;
		}

	}
}
