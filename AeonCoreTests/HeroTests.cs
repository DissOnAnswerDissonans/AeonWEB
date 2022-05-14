namespace Aeon.Core.Tests;

public class HeroTests
{
	const string TS1 = "Test1";
	const string TS2 = "Test2";

	public class TestHero : Hero
	{
		protected override void PostActivate()
		{
			Stats.NewStat(TS1).Set(128);
			Stats.NewStat(TS2).Set(42);
			Stats.SetValue(Health, 1000);
			Stats.SetValue(Attack, 100);
			Stats.SetValue(Magic, 50);

			Shop.AddOffer(TS1, 10, 10);
			Shop.AddOffer(TS1, 110, 95, true);

			Shop.AddOffer(TS2, 2, 20);
			Shop.AddOffer(TS2, 12, 105, true);
		}
	}

	[Fact]
	public void Wage_100then500_Money600()
	{
		Hero hero = new TestHero().Activate();

		Assert.Equal(100, hero.Wage(100));
		Assert.Equal(600, hero.Wage(500));

		Assert.Equal(600, hero.Money);
	}

	[Fact]
	public void Spend_Money500Spend300_Money200()
	{
		Hero hero = new TestHero().Activate();

		Assert.Equal(500, hero.Wage(500));
		Assert.Equal(200, hero.Spend(300));

		Assert.Equal(200, hero.Money);
	}

	[Fact]
	public void Spend_Money500Spend600_throw()
	{
		Hero hero = new TestHero().Activate();

		Assert.Equal(500, hero.Wage(500));
		Assert.Throws<ArgumentException>(() => hero.Spend(600));
	}

	[Fact] 
	public void TryBuyOffer_HaveMoney_retTrue()
	{
		Hero hero = new TestHero().Activate();
		Assert.Equal(120, hero.Wage(120));

		var offer1 = new Offer(TS1, 110, 95, true);
		var offer2 = new Offer(TS2, 2, 20, false);

		Assert.True(hero.TryBuyOffer(offer1));
		Assert.True(hero.TryBuyOffer(offer2));

		Assert.Equal(128 + 110, hero.StatsRO.GetValue(TS1));
		Assert.Equal(42 + 2, hero.StatsRO.GetValue(TS2));
	}

	[Fact]
	public void TryBuyOffer_HaveNotMoney_retFalse()
	{
		Hero hero = new TestHero().Activate();
		Assert.Equal(10, hero.Wage(10));

		var offer1 = new Offer(TS1, 110, 95, true);
		var offer2 = new Offer(TS2, 2, 20, false);

		Assert.False(hero.TryBuyOffer(offer1));
		Assert.False(hero.TryBuyOffer(offer2));

		Assert.Equal(128, hero.StatsRO.GetValue(TS1));
		Assert.Equal(42, hero.StatsRO.GetValue(TS2));
	}

	[Fact]
	public void TryBuyOffer_IncorrectOffer_throw()
	{
		Hero hero = new TestHero().Activate();
		Assert.Equal(10, hero.Wage(10));

		var malOffer = new Offer(TS2, 100500, 1, true);

		Assert.Throws<ArgumentException>(() => hero.TryBuyOffer(malOffer));
	}

	[Fact]
	public void OnBattleStart()
	{
		Hero hero = new TestHero().Activate();
		hero.OnBattleStart(null);
		Assert.Equal(1000, hero.StatsRO.GetDynValue(Hero.Health));
		Assert.Equal(0, hero.StatsRO.GetDynValue(Hero.Income));
	}

	[Fact]
	public void GetDamageTo_null()
	{
		Hero hero = new TestHero().Activate();
		Damage damage = hero.GetDamageTo(null);
		Assert.Equal(hero, damage.Instigator);
		Assert.Equal(100, damage.Phys);
		Assert.Equal(50, damage.Magic);
		Assert.False(damage.IsCrit);
	}

	[Fact]
	public void GetDamageTo_Critical()
	{
		Hero hero = new TestHero().Activate();
		hero.Stats.SetValue(Hero.CritChance, 100);
		Damage damage = hero.GetDamageTo(null);
		Assert.Equal(hero, damage.Instigator);
		Assert.Equal(150, damage.Phys);
		Assert.Equal(50, damage.Magic);
		Assert.True(damage.IsCrit);
	}

	[Fact]
	public void ReceiveDamage_Alive()
	{
		Hero hero = new TestHero().Activate();
		hero.Stats.SetValue(Hero.Block, 10);
		hero.OnBattleStart(null);
		Damage damage = new(null, 100, 50, false);
		hero.ReceiveDamage(damage);
		Assert.Equal(860, hero.StatsRO.GetDynValue(Hero.Health));
		Assert.True(hero.IsAlive);
	}

	[Fact]
	public void ReceiveALotOfDamage_Dead()
	{
		Hero hero = new TestHero().Activate();
		hero.Stats.SetValue(Hero.Block, 10);
		hero.OnBattleStart(null);
		Damage aLotOfDamage = new(null, 1000, 500, false);
		hero.ReceiveDamage(aLotOfDamage);
		Assert.Equal(0, hero.StatsRO.GetDynValue(Hero.Health));
		Assert.False(hero.IsAlive);
	}

	[Fact]
	public void ReceiveALotOfDamage_TankArmor_Alive()
	{
		Hero hero = new TestHero().Activate();
		hero.Stats.SetValue(Hero.Armor, 200);
		hero.OnBattleStart(null);
		Damage aLotOfDamage = new(null, 1000, 500, false);
		hero.ReceiveDamage(aLotOfDamage);
		Assert.True(hero.IsAlive);
	}

	[Fact]
	public void ReceiveALotOfMagicDamage_ArmorPierced_Dead()
	{
		Hero hero = new TestHero().Activate();
		hero.Stats.SetValue(Hero.Armor, 200);
		hero.OnBattleStart(null);
		Damage aLotOfDamage = new(null, 500, 1000, false);
		hero.ReceiveDamage(aLotOfDamage);
		Assert.False(hero.IsAlive);
	}

	[Fact]
	public void AfterHit_Regen()
	{
		Hero hero = new TestHero().Activate();
		hero.Stats.SetValue(Hero.Regen, 50);
		hero.OnBattleStart(null);
		hero.Stats.AddToDynValue(Hero.Health, -500);

		Damage received = new(null, 500, 0, false);
		Damage done = new(null, 500, 0, false);

		hero.AfterHit(received, done);
		Assert.Equal(550, hero.StatsRO.GetDynValue(Hero.Health));
	}

	[Fact]
	public void AfterHit_Blocked_RegenOff()
	{
		Hero hero = new TestHero().Activate();
		hero.Stats.SetValue(Hero.Regen, 50);
		hero.OnBattleStart(null);
		hero.Stats.AddToDynValue(Hero.Health, -500);

		Damage received = new(null, 0, 500, false);
		Damage done = new(null, 0, 500, false);

		hero.AfterHit(received, done);
		Assert.Equal(500, hero.StatsRO.GetDynValue(Hero.Health));
	}
}
