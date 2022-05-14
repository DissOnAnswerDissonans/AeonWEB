using System.Reflection;
using Hero = Aeon.Core.Hero;
using Balance = Aeon.Core.BalanceAttribute;
using System.Linq;

namespace AeonServer.Services;

public class HeroesProvider
{
	public static string GetNameID(Type type) => $"{type.Assembly.GetName().Name}:{type.Name}";

	private IBalanceProvider _balance;

	private Dictionary<string, Type> _types;
	private Dictionary<string, Hero> _heroes;
	private string[] _names;
	public HeroesProvider(IBalanceProvider balance)
	{
		_balance = balance;

		var heroesAssembly = Assembly.Load("Aeon.Heroes");
		_types = heroesAssembly.GetTypes()
				.Where(t => t.BaseType == typeof(Hero))
				//.Select(t => (Hero) Activator.CreateInstance(t))
				.ToDictionary(t => GetNameID(t));
		_heroes = _types.ToDictionary(t => t.Key, t => MakeHero(t.Value)!);
		_names = _heroes.Keys.ToArray();
	}

	public int Total => _heroes.Count;

	internal Hero GetHero(string name) => ((Hero) Activator.CreateInstance(_types[name])!).Activate();
	internal Hero GetHero(int heroID) => GetHero(_names[heroID]);

	internal IReadOnlyList<string> HeroesList => _heroes.Keys.ToList();

	internal HeroInfo[] GetHeroesInfo()
	{
		var test = new HeroInfo[_names.Length];
		for (int i = 0; i < _names.Length; i++) 
			test[i] = GetHeroInfo(i)!;	
		return test;
	}

	internal HeroInfo? GetHeroInfo(int? heroID) => !heroID.HasValue? null : new() { 
		ID = heroID.Value, 
		Name = _names[heroID.Value].Split(':')[1],
		AssemblyName = _names[heroID.Value].Split(':')[0],
	};

	private Hero MakeHero(Type type)
	{
		Hero hero = (Hero) Activator.CreateInstance(type)!;
		hero.GetType()
			.GetRuntimeFields()
			.Select(f => (f, f.GetCustomAttributes(false).Where(a => a is Balance).Cast<Balance>().FirstOrDefault()))
			.Where(x => x.Item2 is not null).ToList().ForEach(a => {
				(FieldInfo f, Balance? b) = a;
				var key = "@" + (b?.BalanceKey ?? f.Name);

				if (f.FieldType == typeof(int))
					f.SetValue(hero, (int) _balance.ValueForHero(hero, key).BaseValue);
				if (f.FieldType == typeof(decimal))
					f.SetValue(hero, _balance.ValueForHero(hero, key).BaseValue);
			});
		hero.Activate();
		return hero;
	}
}
