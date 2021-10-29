using Aeon.Core;

namespace Aeon.Heroes
{
	public class Trickster : Hero
	{
		private const decimal RESET_SALVAGE = 0.80m;
		private int _totalSpent;
		private int ResetMoney => (int) (RESET_SALVAGE * _totalSpent);

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
			ResetStats();
			Wage(ResetMoney);
			_totalSpent = 0;
			return true;
		}

		public override string AbilityText =>
			$"Сбросить всё и получить ${ResetMoney}";
	}
}