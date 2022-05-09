global using System;
//global using Aeon.Base;

namespace Aeon.Core;

public static class Converters
{
	public static Base.OfferData ToBase(this Offer o, IStatContext ctx) => new() {
		Cost = o.Cost,
		StatAmount = new Base.StatData { StatId = o.StatID, RawValue = o.Value, Value = ctx switch {
			null => o.Value,
			_ => StatDiff(o.StatID, ctx, ctx.GetValue(o.StatID), o.Value)
		}},
		IsOpt = o.IsOpt,
	};

	private static decimal StatDiff(string id, IStatContext ctx, int start, int add)
	{
		StatType.Conv conv = ctx[id].Stat.Converter;
		return conv(start + add, ctx) - conv(start, ctx);
	}

	public static int TRound(this decimal d) => (int) d;
}

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class BalanceAttribute : Attribute
{
	public BalanceAttribute() => BalanceKey = null;
	public BalanceAttribute(string key) => BalanceKey = key;
	public string BalanceKey { get; }
}
