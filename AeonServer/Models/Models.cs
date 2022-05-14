using Aeon.Core;
using Humanizer;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using StatData = Aeon.Base.StatData;

namespace AeonServer.Models;


public class PlayerStatus
{
	public string Name { get; set; } = null!;
	public bool IsConnected { get; set; }
}

public class Hero
{
	public int HeroId { get; set; }
	public int Money { get; set; }
	public List<StatData> Stats { get; set; } = null!;

	public static Hero FromAeon(Aeon.Core.Hero hero) => new() { 
		Money = hero.Money, 
		Stats = hero.Stats.All().Select(s => new StatData { 
			StatId = s.Stat.ID, RawValue = s.Value, Value = s.Stat.Converter(s.Value, hero.StatsRO)
		}).ToList() 
	};
}
