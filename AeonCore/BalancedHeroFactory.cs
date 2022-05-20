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
		BalanceAttribute.Activate(hero, _balance);
		hero.Activate(new BalancedShop(_balance));
		return hero;
	}
}
