using Xunit;
using System;

namespace Aeon.Core.Tests
{
	public class StatsContainerTests
	{
		[Fact] public void Register_Kok()
		{
			var stats = new StatsContainer();

			stats.Register<TestStat1>(15);

			Assert.Equal(15, stats.Get<TestStat1>().Value);
		}

		[Fact] public void Register_sameStats_throw()
		{
			var stats = new StatsContainer();

			stats.Register<TestStat1>(15);

			Assert.Throws<InvalidOperationException>(() => stats.Register<TestStat1>(16));
		}

		[Fact] public void Get_afterRegister()
		{
			var stats = new StatsContainer();

			stats.Register<TestStat1>(25);
			stats.Register<TestStat2>(15);

			Assert.Equal(15, stats.Get<TestStat2>().Value);
		}

		[Fact] public void Get_noRegister_throw()
		{
			var stats = new StatsContainer();
			Assert.Throws<InvalidOperationException>(() => stats.Get<Health>());
		}

		[Fact] public void Set_afterRegister_thenGet()
		{
			var stats = new StatsContainer();

			stats.Register<TestStat1>(25);
			stats.Register<TestStat2>(15);

			stats.Set<TestStat1>(15);
			stats.Set<TestStat2>(25);

			Assert.Equal(25, stats.Get<TestStat2>().Value);
		}

		[Fact] public void Set_noRegister_throw()
		{
			var stats = new StatsContainer();
			Assert.Throws<InvalidOperationException>(() => stats.Set<Health>(99));
		}

		[Fact] public void RO_RawValue()
		{
			var stats = new StatsContainer();
			var read = stats as IReadOnlyStats;

			stats.Register<TestStat1>(25);
			stats.Register<TestStat2>(15);

			Assert.Equal(15, read.RawValue<TestStat2>());
		}

		[Fact] public void AddStats()
		{
			var stats = new StatsContainer();
			stats.Register<TestStat1>(25);
			stats.Register<TestStat2>(15);

			var adder = Stat.Make<TestStat1>(5);
			var adder2 = Stat.Make<TestStat2>(7);

			stats.AddStat(adder);
			stats.AddStat(adder2);

			Assert.Equal(30, stats.Get<TestStat1>().Value);
			Assert.Equal(22, stats.Get<TestStat2>().Value);
		}

		[Fact]
		public void AddStats_unregStat_throw()
		{
			var stats = new StatsContainer();
			stats.Register<TestStat1>(25);

			var adder = Stat.Make<TestStat2>(7);

			Assert.Throws<InvalidOperationException>(() => stats.AddStat(adder));
		}
	}
}