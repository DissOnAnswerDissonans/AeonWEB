using Aeon.Core;

namespace Aeon.Heroes
{
	public class Trickster : Hero
	{
		[Balance] private decimal resetSalvage = 0.80m;

		private StatDef Chargeback { get; set; }

		protected override void PostActivate() => Chargeback.Edit.Convert(x => (x * resetSalvage).TRound());


		public override bool TryBuyOffer(Offer offer)
		{
			if (base.TryBuyOffer(offer)) {
				Chargeback.Add(offer.Cost);
				return true;
			}
			return false;
		}

		public override bool UseAbility()
		{
			if (Chargeback == 0) return false;
			var back = Chargeback.Converted.TRound();
			Stats.ResetAll();
			Wage(back);
			return true;
		}

		public override string AbilityText =>
			$"Сбросить всё и получить ${Chargeback.Converted}";
	}
}