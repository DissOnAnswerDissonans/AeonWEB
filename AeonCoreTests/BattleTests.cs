using Moq;
using Xunit;

namespace Aeon.Core.Tests
{
	public class BattleTests
	{
		Battle _battle;
		Mock<IBattler> _battler1, _battler2;

		public BattleTests()
		{
			_battler1 = new Mock<IBattler>();
			_battler1.Setup(x => x.IsAlive).Returns(true);
			_battler2 = new Mock<IBattler>();
			_battler2.Setup(x => x.IsAlive).Returns(true);
			var list = new IBattler[] { _battler1.Object, _battler2.Object };

			var provider = new Mock<IBattle.IBattlersProv>();
			provider.Setup(a => a.GetBattlers()).Returns(list);

			_battle = new Battle(provider.Object, null);
		}

		[Fact]
		public void Battle()
		{
			var damage1 = new Damage
			{ Instigator = _battler1.Object, Phys = 20, Magic = 10, IsCrit = false };
			var d1recieved = damage1.ModMag(a => 0);

			var damage2 = new Damage
			{ Instigator = _battler2.Object, Phys = 10, Magic = 20, IsCrit = false };
			var d2recieved = damage2.ModPhys(a => a - 5);

			_battler1.Setup(x => x.GetDamageTo(It.IsAny<IBattler>())).Returns(damage1);
			_battler2.Setup(x => x.GetDamageTo(It.IsAny<IBattler>())).Returns(damage2);

			int hits = 0;

			_battler1.Setup(x => x.ReceiveDamage(It.Is<Damage>(a => a.Equals(damage2))))
				.Returns(d2recieved)
				.Callback(() => { ++hits; if (hits == 3) _battler1.Setup(a => a.IsAlive).Returns(false); });

			_battler2.Setup(x => x.ReceiveDamage(It.Is<Damage>(a => a.Equals(damage1))))
				.Returns(d1recieved);

			_battler1.Setup(x => x.AfterHit(
				It.Is<Damage>(a => a.Equals(d1recieved)), It.Is<Damage>(a => a.Equals(d2recieved))));
			_battler2.Setup(x => x.AfterHit(
				It.Is<Damage>(a => a.Equals(d2recieved)), It.Is<Damage>(a => a.Equals(d1recieved))));

			_battle.Start();

			Assert.Equal(2, _battle.Winner);
			Assert.Equal(3, _battle.Rounds);
		}
	}
}