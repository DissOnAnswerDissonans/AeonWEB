using Aeon.Core;

namespace Aeon.Heroes
{
	public class Warrior : Hero 
	{
		const decimal CRIT_DMG_BONUS = 0.5m;
		const decimal CRIT_CHA_BONUS = 0.1m;

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
	}
}
