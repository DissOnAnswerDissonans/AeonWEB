using Aeon.Core;
using Aeon.Heroes;
using System;
using System.Collections.Generic;

namespace Aeon.BasicApp
{
	internal static class Info
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

		public struct StatInfo
		{
			public Translation Name { get; init; }
			public Translation Alias { get; init; }
			public Func<decimal, string> ConvFormat { get; init; }
		}

		private static readonly Dictionary<Type, HeroInfo> _heroInfo = new () {
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

		private static readonly Dictionary<Type, StatInfo> _statInfo = new ()
		{
			[typeof(Health)] = new() {
				Name = new("Health", "Здоровье"),
				Alias = new("HP", "ЗДР"),
				ConvFormat = a => $"{a:N0}",
			},
			[typeof(Attack)] = new() {
				Name = new("Attack", "Атака"),
				Alias = new("ATT", "АТК"),
				ConvFormat = a => $"{a:N0}",
			},
			[typeof(Magic)] = new() {
				Name = new("Magic", "Магия"),
				Alias = new("MAG", "МАГ"),
				ConvFormat = a => $"{a:N0}",
			},
			[typeof(CritChance)] = new() {
				Name = new("Crit Chance", "Шанс крита"),
				Alias = new("CRC", "ШКР"),
				ConvFormat = a => $"{a*100:##0.#}%",
			},
			[typeof(CritDamage)] = new() {
				Name = new("Crit Attack", "Крит атака"),
				Alias = new("CAT", "КАТ"),
				ConvFormat = a => $"x{a:#0.##}",
			},
			[typeof(Income)] = new() {
				Name = new("Income", "Прирост"),
				Alias = new("INC", "ПРС"),
				ConvFormat = a => $"{(a-1)*100:##0.#}%",
			},
			[typeof(Block)] = new() {
				Name = new("Block", "Броня"),
				Alias = new("BLK", "БРН"),
				ConvFormat = a => $"{a:N0}",
			},
			[typeof(Armor)] = new() {
				Name = new("Armor", "Защита"),
				Alias = new("ARM", "ЩИТ"),
				ConvFormat = a => $"{a*100:N2}%",
			},
			[typeof(Regen)] = new() {
				Name = new("Regeneration", "Регенерация"),
				Alias = new("REG", "РЕГ"),
				ConvFormat = a => $"{a:N0}",
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

		public static StatInfo AboutStat(Type stat)
		{
			if (!stat.IsSubclassOf(typeof(StatType)))
				throw new ArgumentException("Передан не тип стат", nameof(stat));
			try {
				return _statInfo[stat];
			}
			catch (KeyNotFoundException) {
				return new() {
					Name = new($"?{stat.Name}?", $"?{stat.Name}?"),
					Alias = new("???", "???"),
					ConvFormat = a => $"{a}",
				};
			}
		}
		public static StatInfo AboutStat(StatType stat) => AboutStat(stat.GetType());
		public static StatInfo AboutStat(Stat stat) => AboutStat(stat.StatType.GetType());

		public static string StrStat(Stat stat) => $"{stat.Value} {AboutStat(stat).Alias}";

		public static string StrStatConv(this IReadOnlyStats stats, StatType stat)
			=> AboutStat(stat).ConvFormat(stats[stat].Convert(stats));

		public static string StrOffer(Offer offer) => offer.IsOpt
				? $"опт {offer.Cost,3}$ => {StrStat(offer.Stat)}"
				: $"{offer.Cost,2}$ => {StrStat(offer.Stat)}";
	}
}
