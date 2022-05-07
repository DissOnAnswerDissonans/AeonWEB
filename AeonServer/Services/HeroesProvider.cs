using System.Reflection;
using Hero = Aeon.Core.Hero;

namespace AeonServer.Services;

public class HeroesProvider
{
	private Dictionary<string, Type> _types;
	private Dictionary<string, Hero> _heroes;
	private string[] _names;
	public HeroesProvider()
	{
		var heroesAssembly = Assembly.Load("Aeon.Heroes");
		_types = heroesAssembly.GetTypes()
				.Where(t => t.BaseType == typeof(Hero))
				//.Select(t => (Hero) Activator.CreateInstance(t))
				.ToDictionary(t => t.Name);
		_heroes = _types.ToDictionary(t => t.Key, t => (Hero) Activator.CreateInstance(t.Value)!);
		_names = _heroes.Keys.ToArray();
	}

	public int Total => _heroes.Count;

	internal Hero GetHero(string name) => (Hero) Activator.CreateInstance(_types[name])!;
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
		Name = _names[heroID.Value], 
		AssemblyName = _types[_names[heroID.Value]].Assembly.GetName().Name 
	};
}
