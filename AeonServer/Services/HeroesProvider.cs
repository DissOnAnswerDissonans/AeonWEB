using System.Reflection;
using Hero = Aeon.Core.Hero;
using Balance = Aeon.Core.BalanceAttribute;
using System.Linq;

namespace AeonServer.Services;

public class HeroesProvider
{
	private IBalanceProvider _balance;

	private Dictionary<string, Type> _types;
	private Dictionary<string, Hero> _heroes;
	private string[] _names;
	private Aeon.Core.BalancedHeroFactory _factory;

	public HeroesProvider(IBalanceProvider balance)
	{
		_balance = balance;
		_factory = new(_balance.GetBalanceSheet());

		var heroesAssembly = Assembly.Load("Aeon.Heroes");
		_types = heroesAssembly.GetTypes()
				.Where(t => t.BaseType == typeof(Hero))
				//.Select(t => (Hero) Activator.CreateInstance(t))
				.ToDictionary(t => Hero.GetNameID(t));
		_heroes = _types.ToDictionary(t => t.Key, t => GetHero(t.Value)!);
		_names = _heroes.Keys.ToArray();
	}

	public int Total => _heroes.Count;

	internal Hero GetHero(string name) => GetHero(_types[name]);
	internal Hero GetHero(int heroID) => GetHero(_names[heroID]);
	private Hero GetHero(Type type) => _factory.CreateHero(type);

	internal IReadOnlyList<string> HeroesList => _heroes.Keys.ToList();

	internal HeroInfo[] GetHeroesInfo()
	{
		var test = new HeroInfo[_names.Length];
		for (int i = 0; i < _names.Length; i++) 
			test[i] = GetHeroInfo(i)!;	
		return test;
	}

	internal HeroInfo? GetHeroInfo(int? heroID) => !heroID.HasValue? null 
		: _names[heroID.Value].StartsWith(":") 
		? new() {
			ID = heroID.Value,
			Name = _names[heroID.Value].TrimStart(':'),
			AssemblyName = "",
		}
		: new() { 
			ID = heroID.Value, 
			Name = _names[heroID.Value].Split(':')[1],
			AssemblyName = _names[heroID.Value].Split(':')[0],
		};

}
