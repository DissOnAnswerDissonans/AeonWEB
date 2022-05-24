
using System.Linq;

namespace Aeon.Core;

public static class Converters
{
	public static Base.OfferData ToBase(this Offer o, IStatContext ctx, int id = -1) => new() {
		ID = id,
		Cost = o.Cost,
		StatAmount = new Base.StatData { StatId = o.StatID, RawValue = o.Value, Value = ctx switch {
			null => o.Value,
			_ => StatDiff(o.StatID, ctx, ctx.GetValue(o.StatID), o.Value)
		}},
		IsOpt = o.IsOpt,
	};

	public static Base.Hero FromAeon(Hero hero) => new() {
		HeroId = hero.ID,
		Money = hero.Money,
		Stats = hero.Stats.All().Select(s => new StatData {
			StatId = s.Stat.ID, RawValue = s.Value, Value = s.Stat.Converter(s.Value, hero.StatsRO)
		}).ToList()
	};

	private static decimal StatDiff(string id, IStatContext ctx, int start, int add)
	{
		StatType.Conv conv = ctx[id].Stat.Converter;
		return conv(start + add, ctx) - conv(start, ctx);
	}

	public static int TRound(this decimal d) => (int) d;
	public static decimal Power(this decimal d, int pow) => pow switch {
		0 => 1,
		1 => d,
		< 0 => 1 / Power(d, -pow),
		_ => pow % 2 == 0 ? Power(d * d, pow / 2) : Power(d, pow - 1) * d
	};
}
