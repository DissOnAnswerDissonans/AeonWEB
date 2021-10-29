using Aeon.Core;
using Aeon.Heroes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.BasicApp
{
	static class Info
	{
		public struct Translation
		{
			public string EN { get; init; }
			public string RU { get; init; }

			public Translation(string en, string ru)
			{
				EN = en;
				RU = ru;
			}

			public override string ToString() => Locale switch {
				Lang.EN => EN,
				Lang.RU => RU,
				_ => throw new NotImplementedException(),
			};
		}

		public enum Lang { EN, RU }
		public static Lang Locale { get; set; } = Lang.RU;

		public struct HeroInfo
		{
			public Translation Name { get; init; }
		}

		private static Dictionary<Type, HeroInfo> _heroInfo = new () {
			[typeof(Banker)] = new() {
				Name = new("Banker", "Банкир")
			},
			[typeof(Beast)] = new() {
				Name = new("Beast", "Зверь")
			},
			[typeof(Cheater)] = new() {
				Name = new("Cheater", "Читер")
			},
			[typeof(Fatty)] = new() {
				Name = new("Fatty", "Жиртрест")
			},
			[typeof(Fe11)] = new() {
				Name = new("Fe11", "Фелл")
			},
			[typeof(Warrior)] = new() {
				Name = new("Warrior", "Воин")
			},
			[typeof(Thief)] = new() {
				Name = new("Thief", "Вор")
			},
			[typeof(Master)] = new() {
				Name = new("Master", "Повелитель")
			},
			[typeof(BloodyElf)] = new() {
				Name = new("Bloody Elf", "Кровавый Эльф")
			},
			[typeof(Killer)] = new() {
				Name = new("Killer", "Убийца")
			},
			[typeof(Tramp)] = new() {
				Name = new("Tramp", "Бомж")
			},
			[typeof(Warlock)] = new() {
				Name = new("Warlock", "Чернокнижник")
			},
			[typeof(Rogue)] = new() {
				Name = new("Rogue", "Разбойник")
			},
			[typeof(Vampire)] = new() {
				Name = new("Vampire", "Вампир")
			},
			[typeof(Trickster)] = new() {
				Name = new("Trickster", "Хитрец")
			},
		};

		public static HeroInfo AboutHero(Type hero)
		{
			if (!hero.IsSubclassOf(typeof(Hero)))
				throw new ArgumentException("Передан не герой", nameof(hero));
			try {
				return _heroInfo[hero];
			}
			catch (KeyNotFoundException) {
				return new() {
					Name = new($"?{hero.Name}?", $"?{hero.Name}?")
				};
			}
		}

		public static HeroInfo AboutHero(Hero hero) => AboutHero(hero.GetType());
	}
}
