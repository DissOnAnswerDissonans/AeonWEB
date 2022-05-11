namespace Aeon.Core.Tests;

public class NewStatsTests
{
	const string A = "S1";
	const string B = "S2";
	const string C = "S3";

	[Fact] public void AddStatsToContainer()
	{
		var x = new StatsContainer();
		x.NewStat(A).Should().NotBeNull();
		x.NewStat(B).Should().NotBeNull();
		x.NewStat(C).Should().NotBeNull();
		x.NewStat(C).Should().BeNull();
		x.NewStat("").Should().BeNull();
		x.NewStat(null).Should().BeNull();
	}
	[Fact] public void Getters_Setters()
	{
		var x = new StatsContainer();
		x.NewStat(A);
		x.SetValue(A, 123);
		x.TryGetValue(A).Should().Be(new StatValue { Value = 123 });
		x.GetValue(A).Should().Be(123);
		x.SetValue(B, 456).Should().BeFalse();
		x.TryGetValue(B).Should().BeNull();
		x.Invoking(x => x.GetValue(B)).Should().Throw<KeyNotFoundException>();
	}
	[Fact] public void SC_Setter()
	{
		var x = new StatsContainer();
		x.NewStat(B).Set(100);
		x.GetValue(B).Should().Be(100);
	}
	[Fact] public void Default_ShouldBe0()
	{
		var x = new StatsContainer();
		x.NewStat(A);
		x.GetValue(A).Should().Be(0);
	}
	[Fact] public void AddToStat()
	{
		var x = new StatsContainer();
		x.NewStat(A).Set(123);
		x.AddToValue(A, 321)?.Value.Should().Be(444);
		x.AddToValue(B, 321).Should().BeNull();
	}
	[Fact] public void ResetAll_ResetTo0()
	{
		var x = new StatsContainer();
		x.NewStat(A).Set(123);
		x.ResetAll();
		x.GetValue(A).Should().Be(0);
	}
	[Fact] public void ResetIDs()
	{
		var x = new StatsContainer();
		x.NewStat(A).Set(123);
		x.NewStat(B).Set(123);
		x.Reset(A, B);
		x.GetValue(A).Should().Be(0);
		x.GetValue(B).Should().Be(0);
	}
	[Fact] public void DefaultValue_ShouldSet()
	{
		var x = new StatsContainer();
		x.NewStat(A).Default(100);
		x.GetValue(A).Should().Be(100);
	}
	[Fact] public void DefaultValueAfterNonZeroSetter_ShouldNotSet()
	{
		var x = new StatsContainer();
		x.NewStat(A).Set(123).Default(100);
		x.GetValue(A).Should().Be(123);
		x.NewStat(B).Set(0).Default(100);
		x.GetValue(B).Should().Be(100); // works as intended
	}
	[Fact] public void DefaultValue_ResetToDefault()
	{
		var x = new StatsContainer();
		x.NewStat(A).Default(100);
		x.SetValue(A, 123);
		x.ResetAll();
		x.GetValue(A).Should().Be(100);
	}
	[Fact] public void DefaultFunction()
	{
		var c = new StatsContainer();
		c.NewStat(A).Set(100);
		c.NewStat(B).Default(ctx => ctx.GetValue(A) * 2);
		c.GetValue(B).Should().Be(200);
		c.SetValue(A, 160);
		c.Reset(B);
		c.GetValue(B).Should().Be(320);
	}
	[Fact] public void Limits_Init_ShouldClamp()
	{
		var x = new StatsContainer();
		x.NewStat(A).Limit(100);
		x.NewStat(B).Limit(100, 200);

		x.GetValue(A).Should().Be(0);
		x.GetValue(B).Should().Be(100);
	}
	[Fact] public void Limits_Set_ShouldClamp()
	{
		var x = new StatsContainer();
		x.NewStat(A).Limit(100);
		x.NewStat(B).Limit(200, 300);

		x.SetValue(A, 123);
		x.SetValue(B, 456);

		x.GetValue(A).Should().Be(100);
		x.GetValue(B).Should().Be(300);

		x.SetValue(A, -123);
		x.SetValue(B, 123);

		x.GetValue(A).Should().Be(0);
		x.GetValue(B).Should().Be(200);
	}
	[Fact] public void Limits_Add_ShouldClamp()
	{
		var x = new StatsContainer();
		x.NewStat(A).Limit(100).Set(22);
		x.NewStat(B).Limit(200, 300).Set(222);

		x.AddToValue(A, 100);
		x.AddToValue(B, -100);

		x.GetValue(A).Should().Be(100);
		x.GetValue(B).Should().Be(200);
	}
	[Fact] public void LimitsFunc_Set()
	{
		var c = new StatsContainer();
		c.NewStat(A).Limit(x => (x / 10.0m).TRound() * 10);
		c.SetValue(A, 123);
		c.GetValue(A).Should().Be(120);
	}
	[Fact] public void DefaultConvert_ValueNotChanged()
	{
		var c = new StatsContainer();
		c.NewStat(A);
		c.SetValue(A, 1234);

		c.TryConvert(A).Should().HaveValue();
		c.TryConvert(B).Should().NotHaveValue();

		c.Convert(A).Should().Be(c.GetValue(A));
		c.ConvertAsIs(A).Should().Be((decimal) c.GetValue(A));
	}
	[Fact] public void SetConvert()
	{
		var c = new StatsContainer();
		c.NewStat(A).Convert(x => 1 + x/100m);
		c.SetValue(A, 123);

		c.Convert(A).Should().Be(2);
		c.ConvertAsIs(A).Should().Be(2.23m);
	}
	[Fact] public void LimitsFuncCtx()
	{
		var c = new StatsContainer();
		c.NewStat(A);
		c.NewStat(B).Dependent(A).Limit((x, ctx) => Math.Clamp(x, 0, ctx.GetValue(A)));

		c.SetValue(A, 100);
		c.SetValue(B, 123);
		c.GetValue(B).Should().Be(100);

		c.SetValue(A, 200);
		c.SetValue(B, 123);
		c.GetValue(B).Should().Be(123);
	}
	[Fact] public void LimitsFuncCtxDep_CtxChanged_ValueUpdated()
	{
		var c = new StatsContainer();
		c.NewStat(A);
		c.NewStat(B).Dependent(A).Limit((x, ctx) => Math.Clamp(x, 0, ctx.GetValue(A)));
		c.SetValue(A, 200);
		c.SetValue(B, 123);
		c.SetValue(A, 100);
		c.GetValue(B).Should().Be(100);
	}
	[Fact] public void ConvertFuncCtx()
	{
		var c = new StatsContainer();
		c.NewStat(A);
		c.NewStat(B).Dependent(A).Convert((x, ctx) => x + ctx.GetValue(A));
		c.SetValue(B, 100);
		c.SetValue(A, 200);
		c.Convert(B).Should().Be(300);
	}
	[Fact] public void EditStat()
	{
		var c = new StatsContainer();
		c.NewStat(A).Limit(100).Set(70);

		c.EditStat(A).Limit(50).Convert(x => -x);

		c.Convert(A).Should().Be(-50);
	}
	[Fact] public void CreateDynStat()
	{
		var c = new StatsContainer();
		c.NewStat(A).AddDynamic().Should().NotBeNull();
		c.NewStat(B);

		c.EditStat(B).AddDynamic().Should().NotBeNull();
		c.EditStat(B).AddDynamic().Should().BeNull();
	}
	[Fact] public void Dyn_GettersSetters()
	{
		var c = new StatsContainer();
		c.NewStat(A).AddDynamic();
		c.NewStat(B);
		c.SetDynValue(A, 50);
		c.GetDynValue(A).Should().Be(50);
		c.GetValue($"<DYN>{A}").Should().Be(50);
		c.TryGetDynValue(A).Should().NotBeNull();
		c.TryGetDynValue(B).Should().BeNull();
		c.TryGetDynValue(C).Should().BeNull();
	}
	[Fact] public void Dyn_AddToStat()
	{
		var x = new StatsContainer();
		x.NewStat(A).Set(123).AddDynamic().Set(50).Limit(100);
		x.AddToDynValue(A, 25)?.Value.Should().Be(75);
		x.AddToDynValue(A, 999)?.Value.Should().Be(100);
		x.AddToDynValue(B, 321).Should().BeNull();
	}
	[Fact] public void DynReset()
	{
		var c = new StatsContainer();
		c.NewStat(A).Default(100).Set(200).AddDynamic().Set(100);
		c.NewStat(B).Set(100).AddDynamic().Default(100).Set(200);

		c.ResetDynamic();

		c.GetDynValue(A).Should().Be(0);
		c.GetDynValue(B).Should().Be(100);
	}
	[Fact] public void DefaultDyn_ShouldBe0()
	{
		var c = new StatsContainer();
		c.NewStat(A).AddDynamic();
		c.GetDynValue(A).Should().Be(0);
	}
	[Fact] public void DynAltDefault_ShouldBeA()
	{
		var c = new StatsContainer();
		c.NewStat(A).Set(100).AddDynamic(true);
		c.GetDynValue(A).Should().Be(100);
	}
	[Fact] public void DynDefaultFunction()
	{
		var c = new StatsContainer();
		c.NewStat(A).Set(100).AddDynamic(x => x * 2);
		c.GetDynValue(A).Should().Be(200);
	}
	[Fact] public void DynDependsOfBase()
	{
		var c = new StatsContainer();
		c.NewStat(A).Set(100).AddDynamic(true);
		c.SetValue(A, 200);
		c.ResetDynamic();
		c.GetDynValue(A).Should().Be(200);
	}
	[Fact] public void Collection()
	{
		var c = new StatsContainer();
		c.All().Should().BeEmpty();
		c.NewStat(A).AddDynamic();
		c.NewStat(B);
		c.All().Should().HaveCount(3);
		c.All().Select(x => x.Stat.ID).Should().Contain(A).And.Contain(B).And.Contain($"<DYN>{A}");
		c[A].Stat.ID.Should().Be(A);
		c[A].Stat.DependentIDs.Should().Contain($"<DYN>{A}");
		c.Invoking(c => c[C]).Should().Throw<KeyNotFoundException>();
	}
}