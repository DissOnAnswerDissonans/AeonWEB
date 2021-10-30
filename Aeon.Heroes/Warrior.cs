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
		private const decimal CRIT_DMG_BONUS = 0.5m;
		private const decimal CRIT_CHA_BONUS = 0.1m;

		public class CritDamage : Core.CritDamage
		{
			protected override void Init()
			{
				base.Init();
				Convertor = (a, context) => (a + context.Converted<Magic>() * CRIT_DMG_BONUS) / 100.0m;
			}
		}

		public class CritChance : Core.CritChance
		{
			protected override void Init()
			{
				base.Init();
				Convertor = (a, context) => (a + context.Converted<Magic>() * CRIT_CHA_BONUS) / 100.0m;
			}
		}

		public Warrior()
		{
			Stats.OverrideBehaviour<Core.CritChance, CritChance>();
			Stats.OverrideBehaviour<Core.CritDamage, CritDamage>();
		}

		public override string AbilityText => $"+{StatsRO.Converted<Magic>() * CRIT_DMG_BONUS}% бонус КУР";
	}
}