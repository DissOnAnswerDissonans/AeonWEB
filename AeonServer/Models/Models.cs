using Aeon.Core;
using Humanizer;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Stat = Aeon.Base.Stat;

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
	public List<Stat> Stats { get; set; } = null!;

	public static Hero FromAeon(Aeon.Core.Hero hero) => new() { 
		Money = hero.Money, 
		Stats = hero.Stats.AllStats.Select(s => s.ToBase()).ToList() 
	};
}




public class BattleHero
{ 
	public int HeroId { get; set; }
	public int Health { get; set; }
	public int ExpectedDamage { get; set; }
	public int ExpectedCrit { get; set; }
	public float BoostBonus { get; set; }
}

public class EnemyHero
{
	public int HeroId { get; set; }
	public int Health { get; set; }
}