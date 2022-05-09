using Aeon.Core;

namespace Aeon.Heroes
{
	public class Trickster : Hero
	{
		[Balance] private decimal resetSalvage = 0.80m;

		protected override void PostActivate() { }

		private int _totalSpent;
		private int ResetMoney => (int) (resetSalvage * _totalSpent);

		public override bool TryBuyOffer(Offer offer)
		{
			if (base.TryBuyOffer(offer)) {
				_totalSpent += offer.Cost;
				return true;
			}
			return false;
		}

		public override bool UseAbility()
		{
			if (ResetMoney == 0) return false;
			Stats.ResetAll();
			Wage(ResetMoney);
			_totalSpent = 0;
			return true;
		}

		public override string AbilityText =>
			$"Сбросить всё и получить ${ResetMoney}";
	}
}