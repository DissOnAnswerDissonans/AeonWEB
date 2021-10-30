using System;
using Xunit;

namespace Aeon.Core.Tests
{
	public class TestStat1 : StatType
	{
		protected override void Init() => ID = 1001;
	}

	public class TestStat2 : StatType
	{
		protected override void Init() => ID = 1002;
	}

	public class TestStatCompatable : TestStat1 { }

	public class StatTests
	{
		[Fact]
		public void Make_retStat()
		{
			var stat = Stat.Make<TestStat1>(100);

			Assert.Equal(100, stat.Value);
			Assert.Equal(StatType.Instance<TestStat1>(), stat.StatType);
		}

		[Fact]
		public void Add_statsSameTypes_retStat()
		{
			var stat1 = Stat.Make<TestStat1>(123);
			var stat2 = Stat.Make<TestStat1>(456);

			Stat result = stat1.Add(stat2);
			Stat result2 = stat1 + stat2;

			Assert.Equal(579, result.Value);
			Assert.Equal(result.Value, result2.Value);
			Assert.Equal(StatType.Instance<TestStat1>(), result.StatType);
		}

		[Fact]
		public void Add_statsDifferTypes_throw()
		{
			var stat1 = Stat.Make<TestStat1>(123);
			var stat2 = Stat.Make<TestStat2>(456);

			Assert.Throws<ArgumentException>(() => stat1.Add(stat2));
			Assert.Throws<ArgumentException>(() => stat1 + stat2);
		}

		[Fact]
		public void Add_statsCompatableTypes_retStat()
		{
			var stat1 = Stat.Make<TestStatCompatable>(123);
			var stat2 = Stat.Make<TestStat1>(456);

			Stat result = stat1.Add(stat2);
			Stat result2 = stat1 + stat2;

			Assert.Equal(579, result.Value);
			Assert.Equal(result.Value, result2.Value);
			Assert.Equal(StatType.Instance<TestStatCompatable>(), result.StatType);
		}

		public class ConvStat : StatType
		{
			protected override void Init() => Convertor = (a, c) => a * 2.4m;
		}

		[Fact]
		public void Converted_retDouble()
		{
			var s1 = Stat.Make<ConvStat>(11);

			Assert.Equal(11, s1.Value);
			Assert.Equal(26.4m, s1.Convert(default));
		}

		[Fact]
		public void ConvertedDefault_retDouble()
		{
			var s1 = Stat.Make<TestStat1>(11);

			Assert.Equal(11, s1.Value);
			Assert.Equal(11.0m, s1.Convert(default));
		}
	}
}