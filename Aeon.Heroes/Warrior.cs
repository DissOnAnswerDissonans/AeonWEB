using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Герой средней и поздней стадий игры. Его критический урон
	///	увеличен на 50% от магии, а критический шанс увеличен на
	/// 10% от магии.
	/// </summary>
	public class Warrior : Hero
	{
		[Balance] private decimal critDmgBonus = 0.5m;
		[Balance] private decimal critChaBonus = 0.1m;

		protected override void PostActivate()
		{
			Stats.EditStat(CritDamage).Convert((x, ctx) => x + (ctx.ConvertAsIs(Magic) * critDmgBonus / 100));
			Stats.EditStat(CritChance).Convert((x, ctx) => x + (ctx.ConvertAsIs(Magic) * critChaBonus / 100));
		}

		public override string AbilityText => $"+{StatsRO.Convert(Magic) * critDmgBonus}% бонус КУР";
	}
}