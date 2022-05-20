using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Aeon.Core;

//[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
//public sealed class HeroStatAttribute : Attribute
//{
//	public string StatName { get; }
//	public HeroStatAttribute() => StatName = null;
//	public HeroStatAttribute(string key) => StatName = key;
//	public StatType.Conv Converter { get; set; } = null;
//	public StatValue? Default { get; set; }
//	public (int, int)? Limits { get; set; }

//	internal static void Activate(Hero hero) => hero.GetType()
//		.GetRuntimeProperties()
//		.Select(prop => (prop, prop.GetCustomAttribute<HeroStatAttribute>()))
//		.Where(x => x.Item2 is not null).ToList().ForEach(x => {
//			(PropertyInfo prop, HeroStatAttribute attr) = x;
//			var key = attr.StatName ?? prop.Name;

//			var ctx = hero.Stats.NewStat($"{hero.ID}.{key}");
//			if (attr.Default != null) ctx = ctx.Default(attr.Default.Value);
//			if (attr.Limits != null) ctx =
//				ctx.Limit(attr.Limits.Value.Item1, attr.Limits.Value.Item2);
//			if (attr.Converter != null) ctx = ctx.Convert(attr.Converter);


//		});
//}

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class StatIDAttribute : Attribute
{
	public string ID { get; }
	public StatIDAttribute(string id) => ID = id;
}

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class LimitAttribute : Attribute
{
	public StatType.Limitter Limitter { get; }
	public LimitAttribute(int max) => Limitter = StatType.GetLimitter(max);
	public LimitAttribute(int min, int max) => Limitter = StatType.GetLimitter(min, max);
	public LimitAttribute(Func<int, int> func) => Limitter = (x, ctx) => func(x);
	public LimitAttribute(StatType.Limitter func) => Limitter = func;
}

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class DefaultAttribute : Attribute
{
	public StatType.ContextValue<int> Defaulter { get; }
	public DefaultAttribute(int value) => Defaulter = a => value;
	public DefaultAttribute(StatType.ContextValue<int> func) => Defaulter = func;
}