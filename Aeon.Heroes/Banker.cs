using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Герой предназначен для поздней игры. После каждой
	/// оптовой покупки в Магазине стоимость всех его оптовых
	/// улучшений уменьшается на 1, а после каждого 3-го оптового
	/// улучшения — еще на 1. Суммарное уменьшение стоимости
	/// может достигать 50.
	/// </summary>
	public class Banker : Hero
	{
		private int _optNumber;

		[Balance] private int maxDrop = 50;
		StatDef PriceDrop { get; set; }

		public override bool TryBuyOffer(Offer offer)
		{
			if (!base.TryBuyOffer(offer)) return false;
			if (offer.IsOpt && PriceDrop < maxDrop) {
				int drop = ++_optNumber % 3 == 0 ? 2 : 1;
				Shop.ModifyOffers(o => o.IsOpt, o => new Offer(o.StatID, o.Value, o.Cost - drop, o.IsOpt));
				PriceDrop.Add(drop);
			}
			return true;
		}

		public override string AbilityText => $"Скидка на весь опт ${PriceDrop}";
	}
}