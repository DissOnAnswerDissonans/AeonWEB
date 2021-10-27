using System;
using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Герой ранней стадии игры. Его атака уменьшена на 7%.
	/// Независимо от того, сколько улучшений атаки он
	/// приобретает — у него будет на 7% меньше урона, чем у
	/// пустого героя с аналогичными улучшениями. Во время боя,
	/// если текущее здоровье Героя противника равно
	/// максимальному, то урон Читера умножается на 2.
	/// </summary>
	public class Cheater : Hero
	{
		const decimal ATT_MULTIPLIER = 0.93m;
		const decimal FIRST_ATT_X = 2m;

		public class Attack : Core.Attack
		{
			protected override void Init()
			{
				base.Init();
				Convertor = a => a * ATT_MULTIPLIER;
			}
		}

		public Cheater()
		{
			Stats.OverrideBehaviour<Core.Attack, Attack>();
		}

		public override Damage GetDamageTo(IBattler enemy)
		{
			Damage d = base.GetDamageTo(enemy);
			if (enemy.StatsRO.DynamicValue<Health>() == enemy.StatsRO.ConvInt<Health>()) {
				d = new Damage { Instigator = d.Instigator, IsCrit = d.IsCrit, Magic = d.Magic, Phys = (int) (d.Phys * FIRST_ATT_X) };
			}
			return d;
		}
	}

	public class Warrior : Hero 
	{
		const decimal CRIT_DMG_BONUS = 0.5m;
		const decimal CRIT_CHA_BONUS = 0.1m;

		public class CritDamage : Core.CritDamage
		{
			protected override void Init()
			{
				base.Init();
				Convertor = a => a / 100.0m;
			}
		}
	}
}
