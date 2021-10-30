using Moq;
using System;
using Xunit;

namespace Aeon.Core.Tests
{
	public class ShopTests
	{
		private class TestShop : Shop
		{
			public TestShop()
			{
				AddOffer<TestStat1>(10, 10);
				AddOffer<TestStat1>(100, 100, true);

				AddOffer<TestStat2>(2, 20);
				AddOffer<TestStat2>(5, 45);
				AddOffer<TestStat2>(12, 105, true);
			}
		}

		[Fact]
		public void Custom_TestShop()
		{
			var shop = new TestShop();
			var testOffer = new Offer(Stat.Make<TestStat1>(10), 10, false);
			var testOfferOpt = new Offer(Stat.Make<TestStat1>(100), 100, true);

			Assert.Equal(5, shop.Offers.Count);
			Assert.Contains(testOffer, shop.Offers);
			Assert.Contains(testOfferOpt, shop.Offers);
		}

		[Fact]
		public void ModifyOffers_TestStat2_plus5cost()
		{
			var shop = new TestShop();
			var testOffer1 = new Offer(Stat.Make<TestStat2>(2), 25, false);
			var testOffer2 = new Offer(Stat.Make<TestStat2>(5), 50, false);
			var testOffer3 = new Offer(Stat.Make<TestStat2>(12), 110, true);

			shop.ModifyOffers(
				o => o.Stat.StatType == StatType.Instance<TestStat2>(),
				o => new Offer(o.Stat, o.Cost + 5, o.IsOpt));

			Assert.Contains(testOffer1, shop.Offers);
			Assert.Contains(testOffer2, shop.Offers);
			Assert.Contains(testOffer3, shop.Offers);
		}

		[Fact]
		public void CanBuy_MoneyIsEnough_retTrue()
		{
			var shop = new TestShop();
			var mock = new Mock<IShopper>();
			mock.Setup(x => x.Money).Returns(50);
			mock.Setup(x => x.Shop).Returns(shop);
			var offer = new Offer(Stat.Make<TestStat2>(5), 45, false);

			Assert.True(shop.CanBuy(offer, mock.Object));
		}

		[Fact]
		public void CanBuy_MoneyIsNotEnough_retFalse()
		{
			var shop = new TestShop();
			var mock = new Mock<IShopper>();
			mock.Setup(x => x.Money).Returns(20);
			mock.Setup(x => x.Shop).Returns(shop);
			var offer = new Offer(Stat.Make<TestStat2>(5), 45, false);

			Assert.False(shop.CanBuy(offer, mock.Object));
		}

		[Fact]
		public void CanBuy_IncorrectOffer_throw()
		{
			var shop = new TestShop();
			var mock = new Mock<IShopper>();
			mock.Setup(x => x.Money).Returns(20);
			mock.Setup(x => x.Shop).Returns(shop);
			var offer = new Offer(Stat.Make<TestStat1>(5), 45, false);

			Assert.Throws<ArgumentException>(() => shop.CanBuy(offer, mock.Object));
		}
	}
}