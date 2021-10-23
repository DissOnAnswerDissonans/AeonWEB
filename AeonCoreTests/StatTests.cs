using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AeonCore.Tests
{
	public class TestStat1 : StatType { }
	public class TestStat2 : StatType { }

	public class StatTests
	{
		[Fact] public void Make_retStat()
		{
			Stat stat = Stat.Make<TestStat1>(100);

			Assert.Equal(100, stat.Value);
			Assert.Equal(StatType.Instance<TestStat1>(), stat.StatType);
		}

		[Fact] public void Add_statsSameTypes_retStat()
		{
			Stat stat1 = Stat.Make<TestStat1>(123);
			Stat stat2 = Stat.Make<TestStat1>(456);

			Stat result = stat1.Add(stat2);
			Stat result2 = stat1 + stat2;

			Assert.Equal(579, result.Value);
			Assert.Equal(result.Value, result2.Value);
			Assert.Equal(StatType.Instance<TestStat1>(), result.StatType);
		}

		[Fact] public void Add_statsDifferTypes_throw()
		{
			Stat stat1 = Stat.Make<TestStat1>(123);
			Stat stat2 = Stat.Make<TestStat2>(456);

			Assert.Throws<ArgumentException>(() => stat1.Add(stat2));
			Assert.Throws<ArgumentException>(() => stat1 + stat2);
		}

		public class ConvStat : StatType
		{
			protected override void Init()
			{
				Convertor = a => a * 2.4;
			}
		}

		[Fact] public void Converted_retDouble()
		{
			Stat s1 = Stat.Make<ConvStat>(11);

			Assert.Equal(11, s1.Value);
			Assert.Equal(26.4, s1.Converted);
		}

		[Fact]public void ConvertedDefault_retDouble()
		{
			Stat s1 = Stat.Make<TestStat1>(11);

			Assert.Equal(11, s1.Value);
			Assert.Equal(11.0, s1.Converted);
		}

	}
}
