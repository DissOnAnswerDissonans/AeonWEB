using System.Linq;
using System.Reflection;

namespace Aeon.Core;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class BalanceAttribute : Attribute
{
	public BalanceAttribute() => BalanceKey = null;
	public BalanceAttribute(string key) => BalanceKey = key;
	public string BalanceKey { get; }

	internal static void Activate(Hero hero, BalanceSheet balance)
	{
		hero.GetType()
		.GetRuntimeFields()
		.Select(f => (f, f.GetCustomAttributes(false).Where(a => a is BalanceAttribute).Cast<BalanceAttribute>().FirstOrDefault()))
		.Where(x => x.Item2 is not null).ToList().ForEach(a => {
			(FieldInfo f, BalanceAttribute b) = a;
			var key = "@" + (b?.BalanceKey ?? f.Name);

			if (f.FieldType == typeof(int))
				f.SetValue(hero, (int) balance.HeroesBalance[hero.ID][key].BaseValue);
			if (f.FieldType == typeof(decimal))
				f.SetValue(hero, balance.HeroesBalance[hero.ID][key].BaseValue);
		});
	}
}
