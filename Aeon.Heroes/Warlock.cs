using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Универсальный герой. В течение каждого из посещений
	/// Магазина игрок может единожды использовать способность
	/// Чернокнижника. Использование стоит 10 единиц игровой
	/// валюты. Способность увеличивает игровую валюту на 17
	/// единиц после следующего Боя.
	/// </summary>
	public class Warlock : Hero
	{
		[Balance] private int cost = 10;
		[Balance] private int bonus = 17;

		protected override void PostActivate() { }

		private bool _abilityUsed;

		public override bool UseAbility()
		{
			if (_abilityUsed || Money < cost)
				return false;
			Spend(cost);
			_abilityUsed = true;
			return true;
		}

		public override void AfterBattle(IBattler enemy, bool isWon)
		{
			base.AfterBattle(enemy, isWon);
			if (_abilityUsed) {
				_abilityUsed = false;
				Wage(bonus);
			}
		}

		public override string AbilityText => _abilityUsed
			? $"Вы получите завтра ${bonus}"
			: $"Получить завтра ${bonus} за ${cost}";
	}
}