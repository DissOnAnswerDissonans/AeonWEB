using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Герой поздней стадии игры. У него есть характеристика —
	/// вампиризм. Изначально она равна 15%. После каждого Боя
	/// вампиризм увеличивается на 0.6%. После каждого удара
	/// Повелитель восстанавливает здоровье на 𝑑𝐻 = 𝑑 × 𝑐𝑉,
	/// где dH — восстанавливаемое здоровье, d — урон, cV —
	/// вампиризм.
	/// </summary>
	public class Master : Hero
	{
		[Balance] private decimal vampCoeffStart;
		[Balance] private decimal vampAdder;

		private StatDef VampCoeff { get; set; }
		private StatDef Vamp { get; set; }

		protected override void PostActivate()
		{
			VampCoeff.Edit.Convert(x => vampCoeffStart + vampAdder * x);
			Vamp.Edit.Dependent(Attack, VampCoeff)
				.Convert((x, ctx) => ctx.ConvertAsIs(Attack) * ctx.ConvertAsIs(VampCoeff));
		}

		public override void AfterHit(Damage enemyHit, Damage ourHit)
		{
			base.AfterHit(enemyHit, ourHit);
			Stats.AddToDynValue(Health, Vamp);
		}

		public override void AfterBattle(IBattler enemy, bool isWon)
		{
			base.AfterBattle(enemy, isWon);
			VampCoeff.Add(1);
		}

		public override string AbilityText => $"Вампиризм {VampCoeff:P} ({Vamp})";
	}
}