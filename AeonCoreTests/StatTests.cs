using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AeonCore.Tests
{
	public class StatTests
	{
		[Fact] public void Make_retStat()
		{
			Stat stat = Stat.Make<Health>(100);

			Assert.Equal(100, stat.Value);
			Assert.Equal(StatType.Instance<Health>(), stat.StatType);
		}

		[Fact] public void Add_statsSameTypes_retStat()
		{
			Stat stat1 = Stat.Make<Health>(123);
			Stat stat2 = Stat.Make<Health>(456);

			Stat result = stat1.Add(stat2);
			Stat result2 = stat1 + stat2;

			Assert.Equal(579, result.Value);
			Assert.Equal(result.Value, result2.Value);
			Assert.Equal(StatType.Instance<Health>(), result.StatType);
		}

		[Fact] public void Add_statsDifferTypes_throw()
		{
			Stat stat1 = Stat.Make<Health>(123);
			Stat stat2 = Stat.Make<Attack>(456);

			Assert.Throws<ArgumentException>(() => stat1.Add(stat2));
			Assert.Throws<ArgumentException>(() => stat1 + stat2);
		}
	}
}
