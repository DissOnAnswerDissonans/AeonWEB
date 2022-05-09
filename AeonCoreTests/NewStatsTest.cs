namespace Aeon.Core.Tests;

public class NewStatsTest
{
	[Fact] public void AddStatsToContainer()
	{
		var x = new StatsContainer();
		x.NewStat("s1").Should().NotBeNull();
		x.NewStat("s2").Should().NotBeNull();
		x.NewStat("s3").Should().NotBeNull();
		x.NewStat("s3").Should().BeNull();
		x.NewStat("").Should().BeNull();
		x.NewStat(null).Should().BeNull();
	}

	[Fact] public void SaveStats()
	{
		var x = new StatsContainer();
		x.NewStat("s1");
		x.SetValue("s1", 123);
		x.TryGetValue("s1").Should().Be(123);
		x.SetValue("s2", 456).Should().BeFalse();
		x.TryGetValue("s2").Should().BeNull();
	}

	[Fact] public void AddToStat()
	{
		var x = new StatsContainer();
		x.NewStat("s1");
		x.SetValue("s1", 123);
		x.AddToValue("s1", 321).Should().Be(444);
		x.AddToValue("s2", 321).Should().BeNull();
	}
}
