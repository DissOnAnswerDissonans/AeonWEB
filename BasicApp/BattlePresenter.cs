using Aeon.Core;
using DrawingCLI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Aeon.BasicApp
{
	internal class BattlePresenter : IBattle.ILogger
	{
		private struct LogPT
		{
			internal int Num { get; init; }
			internal IBattle.LogType LogType { get; init; }
			internal int CurrentHP1 { get; init; }
			internal int CurrentHP2 { get; init; }
			internal int MaxHP1 { get; init; }
			internal int MaxHP2 { get; init; }

			internal int RoundNumber { get; init; }
		}

		private readonly Game _game;

		public Battle LastBattle { get; private set; }

		private List<LogPT> _log;

		private List<(Damage dmg1to2, Damage dmg2to1)> _damage;

		private int _round;

		public BattlePresenter(Game game) => _game = game;

		public void StartBattle()
		{
			_log = new();
			_damage = new();
			_round = 0;
			int winner = _game.Battle(out Battle battle, this);
			LastBattle = battle;
			Console.Clear();
			Display();
			Console.ReadKey();
		}

		private void Display()
		{
			var hb1 = new ProgressBar {
				Rect = new Rect { Row = 2, Column = 5, Height = 3, Width = 33 },
				Colors = new Colors { Color = ConsoleColor.Red, BGColor = ConsoleColor.Black },
				FillColor = ConsoleColor.DarkRed
			};

			var hb2 = new ProgressBar {
				Rect = new Rect { Row = 2, Column = 42, Height = 3, Width = 33 },
				Colors = new Colors { Color = ConsoleColor.Red, BGColor = ConsoleColor.Black },
				FillColor = ConsoleColor.DarkRed
			};

			var ground = new DrawRect{
				Rect = new Rect{Row = 23, Column = 0, Height = 2, Width = 80},
				Colors = new Colors {Color = ConsoleColor.DarkGreen, BGColor = ConsoleColor.DarkGreen}
			};

			var log1Zone = new DrawLogZone {
				Rect = new Rect {Row = 8, Column = 6, Height = 11, Width = 8 },
				LeftAligned = false,
				BottomUp = true
			};

			var log2Zone = new DrawLogZone {
				Rect = new Rect {Row = 8, Column = 66, Height = 11, Width = 8 },
				LeftAligned = true,
				BottomUp = true
			};

			foreach (LogPT log in _log) {

				switch (log.LogType) {

				case IBattle.LogType.InitState:
					DrawBattlers();
					UpdateBars(hb1, hb2, log);
					break;

				case IBattle.LogType.AfterDamage:
					DrawAttack();
					UpdateLogZones(log1Zone, log2Zone, log);
					UpdateBars(hb1, hb2, log);
					Console.Beep(200, 200);
					break;

				case IBattle.LogType.AfterHealing:
					UpdateLogZones(log1Zone, log2Zone, log);
					UpdateBars(hb1, hb2, log);
					Console.Beep(400, 200);
					break;

				case IBattle.LogType.AfterBattle:
					DrawScore();
					break;

				}
				ground.Draw();
				Print.Pos(39, 3, $"{log.Num,2}");
				Console.SetCursorPosition(0, 0);
				Thread.Sleep(200);
			}
		}

		private void UpdateBars(ProgressBar hb1, ProgressBar hb2, LogPT log)
		{
			hb1.SetValues(log.CurrentHP1, log.MaxHP1);
			hb2.SetValues(log.CurrentHP2, log.MaxHP2);

			hb1.Draw();
			hb2.Draw();
			Console.SetCursorPosition(0, 0);
		}

		private void UpdateLogZones(DrawLogZone zone1, DrawLogZone zone2, LogPT log)
		{
			ConsoleColor color1 = ConsoleColor.Green;
			ConsoleColor color2 = ConsoleColor.Green;
			if (log.LogType == IBattle.LogType.AfterDamage) {
				color1 = _damage[_round].dmg2to1.IsCrit ? ConsoleColor.DarkYellow : ConsoleColor.Red;
				color2 = _damage[_round].dmg1to2.IsCrit ? ConsoleColor.DarkYellow : ConsoleColor.Red;
				_round++;
			}

			int z1 = log.CurrentHP1 -_log[log.Num-1].CurrentHP1;
			zone1.Add($"{z1:+#.#;-#.#}", color1);

			int z2 = log.CurrentHP2 - _log[log.Num-1].CurrentHP2;
			zone2.Add($"{z2:+#.#;-#.#}", color2);

			zone1.Draw();
			zone2.Draw();
		}

		public void LogBattlersState(IBattler battler1, IBattler battler2, IBattle.LogType logType)
		{
			_log.Add(new LogPT {
				Num = _log.Count,
				LogType = logType,
				CurrentHP1 = battler1.StatsRO.DynamicValue<Health>(),
				CurrentHP2 = battler2.StatsRO.DynamicValue<Health>(),
				MaxHP1 = battler1.StatsRO.ConvInt<Health>(),
				MaxHP2 = battler2.StatsRO.ConvInt<Health>(),
			});
		}

		public void LogDamage(Damage dmg1to2, Damage dmg2to1) => _damage.Add((dmg1to2, dmg2to1));

		private void DrawBattlers()
		{
			byte[] bytes1 = new byte[128];
			byte[] bytes2 = new byte[128];
			for (byte i = 0; i < 128; ++i) {
				bytes1[i] = i;
				bytes2[i] = (byte) (i + 128);
			}
			ColorPic pic1 = new(16, 8, bytes1);
			ColorPic pic2 = new(16, 8, bytes2);
			pic1.DrawIn(16, 15);
			pic2.DrawIn(48, 15);
		}

		private void DrawScore()
		{
			byte[] bytes = new byte[] { 246, 111, 0, 246, 111 };
			var pic = new SimplePic(4, 10, new BitArray(bytes));
			var pic1 = new SimplePic(8, 10, new BitArray(numbers[_game.Player1.Score]));
			var pic2 = new SimplePic(8, 10, new BitArray(numbers[_game.Player2.Score]));
			var colors = new Colors { Color = ConsoleColor.White, BGColor = ConsoleColor.Black };
			var plusColors = new Colors { Color = ConsoleColor.DarkGreen, BGColor = ConsoleColor.Black };
			pic.DrawIn(38, 6, colors);
			pic1.DrawIn(30, 6, LastBattle.Winner == 1 ? plusColors : colors);
			pic2.DrawIn(44, 6, LastBattle.Winner == 2 ? plusColors : colors);
		}

		private void DrawAttack()
		{
			foreach (byte[] a in _anim) {
				var pic = new SimplePic(16, 8, new BitArray(a));
				pic.DrawIn(32, 15, new Colors { Color = ConsoleColor.Red, BGColor = ConsoleColor.Black });
				Thread.Sleep(50);
			}
		}

		private readonly byte[][] _anim = new byte[][] {
			new byte[] { 3, 192, 3, 192, 3, 192, 3, 192, 3, 192, 3, 192, 3, 192, 3, 192 },
			new byte[] { 128, 1, 192, 3, 96, 6, 48, 12, 24, 24, 12, 48, 7, 224, 3, 192 },
			new byte[] { 7, 224, 28, 56, 112, 14, 192, 3, 192, 3, 112, 14, 31, 248, 7, 224},
			new byte[] { 0,0,0,0,0,0,0,0,0,0,0,0,255,255, 255, 255 },
			new byte[] { 0,0,0,0,0,0,0,0,0,0,0,0,15,240, 15, 240 },
			new byte[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
		};

		private readonly byte[][] numbers = new byte[][] {
			new byte[] { 63, 63, 51, 51, 51, 51, 51, 51, 63, 63 }, // 0
			new byte[] { 63, 63, 12, 12, 12, 12, 12, 12, 63, 63 }, // 1
			new byte[] { 63, 63, 48, 48, 63, 63, 03, 03, 63, 63,}, // 2
			new byte[] { 63, 63, 48, 48, 63, 63, 48, 48, 63, 63 }, // 3
			new byte[] { 51, 51, 51, 51, 63, 63, 48, 48, 48, 48 }, // 4
			new byte[] { 63, 63, 03, 03, 63, 63, 48, 48, 63, 63 }, // 5
			new byte[] { 63, 63, 03, 03, 63, 63, 51, 51, 63, 63 }, // 6
			new byte[] { 63, 63, 48, 48, 48, 48, 48, 48, 48, 48 }, // 7
			new byte[] { 63, 63, 51, 51, 63, 63, 51, 51, 63, 63 }, // 8
			new byte[] { 63, 63, 51, 51, 63, 63, 48, 48, 63, 63 }, // 9
		};
	}
}
