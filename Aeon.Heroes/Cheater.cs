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
		private const decimal ATT_MULTIPLIER = 0.93m;
		private const decimal FIRST_ATT_X = 2m;

		public class Attack : Core.Attack
		{
			protected override void Init()
			{
				base.Init();
				Convertor = (a, context) => a * ATT_MULTIPLIER;
			}
		}

		public Cheater() => Stats.OverrideBehaviour<Core.Attack, Attack>();

		public override Damage GetDamageTo(IBattler enemy)
		{
			Damage d = base.GetDamageTo(enemy);
			if (enemy.StatsRO.DynamicValue<Health>() == enemy.StatsRO.ConvInt<Health>()) {
				d = new Damage { Instigator = d.Instigator, IsCrit = d.IsCrit, Magic = d.Magic, Phys = (int) (d.Phys * FIRST_ATT_X) };
			}
			return d;
		}

		public override string AbilityText => $"Первый удар: {base.GetDamageTo(default).Phys * FIRST_ATT_X} физ. урона";
	}
}