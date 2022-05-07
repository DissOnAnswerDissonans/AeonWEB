using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Герой средней и поздней стадий игры. Его критический урон
	///	увеличен на 50% от магии, а критический шанс увеличен на
	/// 10% от магии.
	/// </summary>
	//public class Warrior : Hero
	//{
	//	[Balance] private decimal critDmgBonus = 0.5m;
	//	[Balance] private decimal critChaBonus = 0.1m;

	//	public class CritDamage : Core.CritDamage
	//	{
	//		protected override void Init()
	//		{
	//			base.Init();
	//			Convertor = (a, context) => (a + context.Converted<Magic>() * critDmgBonus) / 100.0m;
	//		}
	//	}

	//	public class CritChance : Core.CritChance
	//	{
	//		protected override void Init()
	//		{
	//			base.Init();
	//			Convertor = (a, context) => (a + context.Converted<Magic>() * critChaBonus) / 100.0m;
	//		}
	//	}

	//	public Warrior()
	//	{
	//		Stats.OverrideBehaviour<Core.CritChance, CritChance>();
	//		Stats.OverrideBehaviour<Core.CritDamage, CritDamage>();
	//	}

	//	public override string AbilityText => $"+{StatsRO.Converted<Magic>() * critDmgBonus}% бонус КУР";
	//}
}