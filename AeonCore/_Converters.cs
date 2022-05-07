using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Core;

public static class Converters
{
	public static Base.Offer ToBase(this Offer o) => new() {
		Cost = o.Cost,
		StatAmount = o.Stat.ToBase(),
		IsOpt = o.IsOpt,
	};

	public static Base.Stat ToBase(this Stat s) => new() {
		StatId = s.StatType.ID,
		RawValue = s.Value,
		Value = s.Convert(default)
	};
}
