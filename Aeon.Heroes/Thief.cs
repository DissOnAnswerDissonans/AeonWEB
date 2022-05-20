using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Герой ранней и средней стадий игры. После каждого удара
	/// максимальное здоровье Вора увеличивается, а здоровье
	/// противника уменьшается на 1+w, где w — количество побед Вора.
	/// Сука, он мне всю инкапсуляцию поломал, негодяй!
	/// </summary>
	public class Thief : Hero
	{
		protected override void PostActivate() { }

		private StatDef StealAmount { get; set; }
		private StatDef Stolen { get; set; }

		public override Damage GetDamageTo(IBattler enemy)
		{
			(enemy as Hero)?.Stats.AddToValue(Health, -StealAmount);
			Stats.AddToValue(Health, +StealAmount);
			Stolen.Add(StealAmount);
			return base.GetDamageTo(enemy);
		}

		public override void AfterBattle(IBattler enemy, bool isWon)
		{
			base.AfterBattle(enemy, isWon);
			if (isWon) StealAmount.Add(1);
		}

		public override string AbilityText => $"Спижжено {Stolen} здоровья";
	}
}