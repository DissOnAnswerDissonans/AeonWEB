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
		private const int COST = 10;
		private const int BONUS = 17;

		private bool _abilityUsed;

		public override bool UseAbility()
		{
			if (_abilityUsed || Money < COST)
				return false;
			Spend(COST);
			_abilityUsed = true;
			return true;
		}

		public override void AfterBattle(IBattler enemy, bool isWon)
		{
			base.AfterBattle(enemy, isWon);
			if (_abilityUsed) {
				_abilityUsed = false;
				Wage(BONUS);
			}
		}

		public override string AbilityText => _abilityUsed
			? $"Вы получите завтра ${BONUS}"
			: $"Получить завтра ${BONUS} за ${COST}";
	}
}