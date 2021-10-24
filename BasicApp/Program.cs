using System;
using System.Collections.Generic;
using System.Linq;
using Aeon.Core;
using DrawingCLI;

namespace Aeon.BasicApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello Aeon!");

			Player player1 = new Player();
			Player player2 = new Player();

			Game game = new Game(player1, player2);
			ShopPresenter presenter = new(game);

			presenter.SetPlayer(1);
			presenter.Display();


			//Console.Clear();

			//DrawingCLI.Table table = new Table();
			//table.AddColumns(20, 6, 15, 24);
			//table.AddRows(9);
			//table.SetPos(6, 3);
			//table.Draw();

			Console.ReadKey();
		}
	}

	class ShopPresenter
	{
		private Game _game;
		internal Shop Shop { get; private set; }
		internal Hero Hero { get; private set; }

		public ShopPresenter(Game game)
		{
			_game = game;
		}

		private Dictionary<StatType, List<Offer>> _offers;

		internal void Display()
		{
			var offers = from offer in Shop.Offers.ToList()
						 orderby offer.Cost
						 group offer by offer.Stat.StatType into statGroup
						 orderby statGroup.Key.DebugNames.AliasEN
						 select statGroup;

			_offers = offers.ToDictionary(a => a.Key, a => a.ToList());

			Console.Clear();

			var tabStats = new Table();
			tabStats.SetPos(7, 2);
			tabStats.AddColumns(19, 6);
			tabStats.AddRows(9);

			var tabOffers = new Table();
			tabOffers.SetPos(34, 2);
			tabOffers.AddColumns(15, 21);
			tabOffers.AddRows(9);

			int rowN = 0;
			foreach (StatType v in _offers.Keys) {
				tabStats[rowN, 0] = $" {v.DebugNames.FullNameRU, 17} ";
				tabStats[rowN, 1] = $" {Hero.StatsRO[v].Value, 4} ";
				tabOffers[rowN, 0] = $" {_offers[v][0]} ";
				tabOffers[rowN, 1] = $" {_offers[v][1]} ";
				++rowN;
			}

			tabStats.Draw();
			tabOffers.Draw();
		}

		//private void WriteDv(int type)
		//{
		//	string dv = type switch
		//	{
		//		01 => "├──────────────────┼──────╫───────────────┼─────────────────────┤",
		//		02 => "╟──────────────────┼──────╫───────────────┼─────────────────────╢",

		//		11 => "┌──────────────────┬──────╥───────────────┬─────────────────────┐",
		//		12 => "╔══════════════════╤══════╦═══════════════╤═════════════════════╗",

		//		21 => "└──────────────────┴──────╨───────────────┴─────────────────────┘",
		//		22 => "╚═════════╦════════╪══════╬═══════════════╧═════════════════════╝",
		//	};
		//	Console.WriteLine(dv);
		//}

		internal Shop SetPlayer(int playerID) {
			Hero = _game.GetHero(playerID);
			return Shop = Hero.Shop;
		}
	}
}
