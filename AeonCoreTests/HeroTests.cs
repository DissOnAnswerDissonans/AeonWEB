using System;
using Xunit;

namespace Aeon.Core.Tests
{
	public class HeroTests
	{
		public class TestHero : Hero
		{
			internal TestHero()
			{
				Stats.Register<TestStat1>(128);
				Stats.Register<TestStat2>(42);

				Stats.Set<Health>(1000);
				Stats.Set<Attack>(100);
				Stats.Set<Magic>(50);

				Shop = new TestShop();
			}

			private class TestShop : Shop
			{
				internal TestShop()
				{
					AddOffer<TestStat1>(10, 10);
					AddOffer<TestStat1>(110, 95, true);

					AddOffer<TestStat2>(2, 20);
					AddOffer<TestStat2>(12, 105, true);
				}
			}
		}

		[Fact]
		public void Wage_100then500_Money600()
		{
			Hero hero = new TestHero();

			Assert.Equal(100, hero.Wage(100));
			Assert.Equal(600, hero.Wage(500));

			Assert.Equal(600, hero.Money);
		}

		[Fact]
		public void Spend_Money500Spend300_Money200()
		{
			Hero hero = new TestHero();

			Assert.Equal(500, hero.Wage(500));
			Assert.Equal(200, hero.Spend(300));

			Assert.Equal(200, hero.Money);
		}

		[Fact]
		public void Spend_Money500Spend600_throw()
		{
			Hero hero = new TestHero();

			Assert.Equal(500, hero.Wage(500));
			Assert.Throws<ArgumentException>(() => hero.Spend(600));
		}

		[Fact]
		public void TryBuyOffer_HaveMoney_retTrue()
		{
			Hero hero = new TestHero();
			Assert.Equal(120, hero.Wage(120));

			var offer1 = new Offer(Stat.Make<TestStat1>(110), 95, true);
			var offer2 = new Offer(Stat.Make<TestStat2>(2), 20, false);

			Assert.True(hero.TryBuyOffer(offer1));
			Assert.True(hero.TryBuyOffer(offer2));

			Assert.Equal(128 + 110, hero.StatsRO.RawValue<TestStat1>());
			Assert.Equal(42 + 2, hero.StatsRO.RawValue<TestStat2>());
		}

		[Fact]
		public void TryBuyOffer_HaveNotMoney_retFalse()
		{
			Hero hero = new TestHero();
			Assert.Equal(10, hero.Wage(10));

			var offer1 = new Offer(Stat.Make<TestStat1>(110), 95, true);
			var offer2 = new Offer(Stat.Make<TestStat2>(2), 20, false);

			Assert.False(hero.TryBuyOffer(offer1));
			Assert.False(hero.TryBuyOffer(offer2));

			Assert.Equal(128, hero.StatsRO.RawValue<TestStat1>());
			Assert.Equal(42, hero.StatsRO.RawValue<TestStat2>());
		}

		[Fact]
		public void TryBuyOffer_IncorrectOffer_throw()
		{
			Hero hero = new TestHero();
			Assert.Equal(10, hero.Wage(10));

			var malOffer = new Offer(Stat.Make<TestStat2>(100500), 1, true);

			Assert.Throws<ArgumentException>(() => hero.TryBuyOffer(malOffer));
		}

		[Fact]
		public void OnBattleStart()
		{
			Hero hero = new TestHero();
			hero.OnBattleStart(null);
			Assert.Equal(1000, hero.StatsRO.DynamicValue<Health>());
			Assert.Equal(0, hero.StatsRO.DynamicValue<Income>());
		}

		[Fact]
		public void GetDamageTo_null()
		{
			Hero hero = new TestHero();
			Damage damage = hero.GetDamageTo(null);
			Assert.Equal(hero, damage.Instigator);
			Assert.Equal(100, damage.Phys);
			Assert.Equal(50, damage.Magic);
			Assert.False(damage.IsCrit);
		}

		[Fact]
		public void GetDamageTo_Critical()
		{
			Hero hero = new TestHero();
			hero.Stats.Set<CritChance>(100);
			Damage damage = hero.GetDamageTo(null);
			Assert.Equal(hero, damage.Instigator);
			Assert.Equal(150, damage.Phys);
			Assert.Equal(50, damage.Magic);
			Assert.True(damage.IsCrit);
		}

		[Fact]
		public void ReceiveDamage_Alive()
		{
			Hero hero = new TestHero();
			hero.Stats.Set<Block>(10);
			hero.OnBattleStart(null);
			Damage damage = new(null, 100, 50, false);
			hero.ReceiveDamage(damage);
			Assert.Equal(860, hero.StatsRO.DynamicValue<Health>());
			Assert.True(hero.IsAlive);
		}

		[Fact]
		public void ReceiveALotOfDamage_Dead()
		{
			Hero hero = new TestHero();
			hero.Stats.Set<Block>(10);
			hero.OnBattleStart(null);
			Damage aLotOfDamage = new(null, 1000, 500, false);
			hero.ReceiveDamage(aLotOfDamage);
			Assert.Equal(0, hero.StatsRO.DynamicValue<Health>());
			Assert.False(hero.IsAlive);
		}

		[Fact]
		public void ReceiveALotOfDamage_TankArmor_Alive()
		{
			Hero hero = new TestHero();
			hero.Stats.Set<Armor>(200);
			hero.OnBattleStart(null);
			Damage aLotOfDamage = new(null, 1000, 500, false);
			hero.ReceiveDamage(aLotOfDamage);
			Assert.True(hero.IsAlive);
		}

		[Fact]
		public void ReceiveALotOfMagicDamage_ArmorPierced_Dead()
		{
			Hero hero = new TestHero();
			hero.Stats.Set<Armor>(200);
			hero.OnBattleStart(null);
			Damage aLotOfDamage = new(null, 500, 1000, false);
			hero.ReceiveDamage(aLotOfDamage);
			Assert.False(hero.IsAlive);
		}

		[Fact]
		public void AfterHit_Regen()
		{
			Hero hero = new TestHero();
			hero.Stats.Set<Regen>(50);
			hero.OnBattleStart(null);
			hero.Stats.ModifyDyn<Health>(-500);

			Damage received = new(null, 500, 0, false);
			Damage done = new(null, 500, 0, false);

			hero.AfterHit(received, done);
			Assert.Equal(550, hero.StatsRO.DynConvInt<Health>());
		}

		[Fact]
		public void AfterHit_Blocked_RegenOff()
		{
			Hero hero = new TestHero();
			hero.Stats.Set<Regen>(50);
			hero.OnBattleStart(null);
			hero.Stats.ModifyDyn<Health>(-500);

			Damage received = new(null, 0, 500, false);
			Damage done = new(null, 0, 500, false);

			hero.AfterHit(received, done);
			Assert.Equal(500, hero.StatsRO.DynConvInt<Health>());
		}
	}
}