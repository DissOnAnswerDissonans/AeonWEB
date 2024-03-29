﻿using Aeon.Core;

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
		[Balance] private decimal attMultiplier = 0.93m;
		[Balance] private decimal firstAttX = 2m;

		protected override void PostActivate() => 
			Stats.EditStat(Attack).Convert((x, ctx) => x * attMultiplier);

		public override Damage GetDamageTo(IBattler enemy)
		{
			Damage d = base.GetDamageTo(enemy);
			if (enemy.StatsRO.GetDynValue(Health) == enemy.StatsRO.Convert(Health)) {
				d = new Damage { Instigator = d.Instigator, IsCrit = d.IsCrit, Magic = d.Magic, Phys = (int) (d.Phys * firstAttX) };
			}
			return d;
		}

		public override string AbilityText => $"Первый удар: {base.GetDamageTo(default).Phys * firstAttX} физ. урона";
	}
}