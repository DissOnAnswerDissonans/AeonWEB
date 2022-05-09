using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Герой ранней и средней стадий игры. Его улучшения дают
	/// на 9% больше здоровья. После каждого Боя он получает +2
	/// регенерации.
	/// </summary>
	public class Fatty : Hero
	{
		[Balance] private decimal healthMultiplier = 1.095m;
		[Balance] private int regenBonus = 2;

		protected override void PostActivate() => 
			Shop.ModifyOffers(o => o.StatID == Health, o => o with { Value = (o.Value.Value * healthMultiplier).TRound() } );

		private int _addedRegen;
		public override string AbilityText => $"Нажрал +{_addedRegen} регенерации";

		public override void AfterBattle(IBattler enemy, bool isWon)
		{
			base.AfterBattle(enemy, isWon);
			Stats.AddToValue(Regen, regenBonus);
			_addedRegen += regenBonus;
		}
	}
}