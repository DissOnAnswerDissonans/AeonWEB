﻿using Aeon.Core;
using DrawingCLI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aeon.BasicApp
{
	internal class ShopPresenter
	{
		private readonly Game _game;
		internal Shop Shop { get; private set; }
		internal Hero Hero { get; private set; }

		public ShopPresenter(Game game)
		{
			_game = game;

			tabStats.SetPos(7, 2);
			tabStats.AddColumns(19, 6);
			tabStats.AddRows(9);

			tabOffers.SetPos(34, 2);
			tabOffers.AddColumns(15, 21);
			tabOffers.AddRows(9);

			moneyBox = new DrawTextRect(new DrawRect() {
				Colors = new() { Color = ConsoleColor.Yellow, BGColor = ConsoleColor.Black },
				Rect = new() { Row = 21, Column = 10, Height = 3, Width = 10 }
			}, $"$0000");

			abilityBox = new DrawTextRect(new DrawRect() {
				Rect = new() { Row = 21, Column = 21, Height = 3, Width = 34 }
			}, $"---Способность---");

			endShopBox = new DrawTextRect(new DrawRect() {
				Rect = new() { Row = 21, Column = 56, Height = 3, Width = 14 }
			}, $"Конец хода");
		}

		private Dictionary<StatType, List<Offer>> _offers;

		private readonly Table<string> tabStats = new();
		private readonly Table<Offer> tabOffers = new();

		private readonly DrawTextRect moneyBox;
		private readonly DrawTextRect abilityBox;
		private readonly DrawTextRect endShopBox;

		private int _selectorX, _selectorY;

		internal void FullDraw()
		{
			Console.Clear();

			Print.Pos(3, 1, Info.AboutHero(Hero).Name);

			SetTableValues();
			SetColors();

			tabStats.Draw();
			tabOffers.Draw();
			moneyBox.Draw();
			abilityBox.Draw();
			endShopBox.Draw();
		}

		internal void Display()
		{
			SetTableValues();
			SetColors();
			moneyBox.Text = $"$ {Hero.Money,-4}";
			abilityBox.Text = Hero.AbilityText;

			tabStats.DrawTextOnly();
			tabOffers.DrawTextOnly();
			moneyBox.Draw();
			abilityBox.Draw();
			endShopBox.Draw();

			Console.SetCursorPosition(0, 0);
		}

		private void SetTableValues()
		{
			IOrderedEnumerable<IGrouping<StatType, Offer>> offers = from offer in Shop.Offers.ToList()
																	group offer by offer.Stat.StatType into statGroup
																	orderby statGroup.Key.ID
																	select statGroup;

			_offers = offers.ToDictionary(a => a.Key, a => a.ToList());

			int rowN = 0;
			foreach (StatType v in _offers.Keys) {
				tabStats[rowN, 0] = $"{Hero.StatsRO.StrStatConv(v),-6}{Info.AboutStat(v).Name,11}";
				tabStats[rowN, 1] = $"{Hero.StatsRO[v].Value,4}";

				tabOffers[rowN, 0, Info.StrOffer(_offers[v][0])] = _offers[v][0];
				tabOffers[rowN, 1, Info.StrOffer(_offers[v][1])] = _offers[v][1];
				++rowN;
			}
		}

		private void SetColors()
		{
			tabOffers.SetTextColorIf(c => c.Cost > Hero.Money, ConsoleColor.Red);
			tabOffers.SetTextColorIf(c => c.Cost <= Hero.Money, ConsoleColor.White);

			tabOffers.SetTextColor(_selectorY, _selectorX, ConsoleColor.Black,
				tabOffers[_selectorY, _selectorX].Cost > Hero.Money ? ConsoleColor.Red : ConsoleColor.White);

			abilityBox.Colors = (_selectorY == 9 && _selectorX == 0)
				? new Colors { Color = ConsoleColor.Green, BGColor = ConsoleColor.DarkGreen }
				: new Colors { Color = ConsoleColor.DarkGreen, BGColor = ConsoleColor.Black };

			endShopBox.Colors = (_selectorY == 9 && _selectorX == 1)
				? new Colors { Color = ConsoleColor.Red, BGColor = ConsoleColor.DarkRed }
				: new Colors { Color = ConsoleColor.DarkRed, BGColor = ConsoleColor.Black };
		}

		internal Shop SetPlayer(int playerID)
		{
			Hero = _game.GetHero(playerID);
			return Shop = Hero.Shop;
		}

		internal bool Input(ConsoleKeyInfo info)
		{
			switch (info.Key) {
			case ConsoleKey.UpArrow:
				_selectorY -= 1; break;
			case ConsoleKey.DownArrow:
				_selectorY += 1; break;
			case ConsoleKey.LeftArrow:
				_selectorX -= 1; break;
			case ConsoleKey.RightArrow:
				_selectorX += 1; break;
			case ConsoleKey.Enter:
				if (_selectorY == 9) {
					if (_selectorX == 1) {
						return false;
					}
					if (_selectorX == 0) {
						Hero.UseAbility();
					}
					break;
				}
				Hero.TryBuyOffer(tabOffers[_selectorY, _selectorX]); break;
			};
			_selectorY = Math.Clamp(_selectorY, 0, 9);
			_selectorX = Math.Clamp(_selectorX, 0, 1);
			return true;
		}

		internal void EnterShopRoutine(int player)
		{
			SetPlayer(player);
			tabStats.SetColor(Program.PlayerColors[player - 1]);
			tabOffers.SetColor(Program.PlayerColors[player - 1]);
			FullDraw();

			do {
				Display();
			} while (Input(Console.ReadKey()));
		}
	}
}