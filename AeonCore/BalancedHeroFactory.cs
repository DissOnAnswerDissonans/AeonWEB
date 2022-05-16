using Aeon.Base;
using System.Linq;
using System.Reflection;

namespace Aeon.Core;

public class BalancedHeroFactory
{
	private BalanceSheet _balance;
	public BalancedHeroFactory(BalanceSheet balance)
	{
		_balance = balance;
	}

	public Hero CreateHero(Type type)
	{
		Hero hero = (Hero) Activator.CreateInstance(type);
		hero.GetType()
		.GetRuntimeFields()
		.Select(f => (f, f.GetCustomAttributes(false).Where(a => a is BalanceAttribute).Cast<BalanceAttribute>().FirstOrDefault()))
		.Where(x => x.Item2 is not null).ToList().ForEach(a => {
			(FieldInfo f, BalanceAttribute b) = a;
			var key = "@" + (b?.BalanceKey ?? f.Name);

			if (f.FieldType == typeof(int))
				f.SetValue(hero, (int) _balance.HeroesBalance[hero.ID][key].BaseValue);
			if (f.FieldType == typeof(decimal))
				f.SetValue(hero, _balance.HeroesBalance[hero.ID][key].BaseValue);
		});
		hero.Activate(new BalancedShop(_balance));
		return hero;
	}
}
