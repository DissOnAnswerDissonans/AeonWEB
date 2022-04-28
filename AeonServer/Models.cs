using System.ComponentModel.DataAnnotations;
namespace AeonServer;


public class HeroModel
{
	public int HeroId { get; set; }
	public int Money { get; set; }
	public List<StatModel> Stats { get; set; } = null!;
}

public class OfferModel
{
	public StatModel StatAmount { get; set; } = null!;
	public int Cost { get; set; }
}

public class StatModel
{
	public byte StatId { get; set; }
	public int RawValue { get; set; }
	public int Value { get; set; }
}

public class BattleHeroModel 
{ 
	public int HeroId { get; set; }
	public int Health { get; set; }
	public int ExpectedDamage { get; set; }
	public int ExpectedCrit { get; set; }
	public float BoostBonus { get; set; }
}

public class EnemyHeroModel
{
	public int HeroId { get; set; }
	public int Health { get; set; }
}