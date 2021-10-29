using Aeon.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Heroes
{
	/// <summary>
	/// Герой средней и поздней стадии игры.Способность Вампира
	/// улучшаемая. Изначальный уровень способности 1. В
	/// Магазине за 100 единиц игровой валюты способность можно
	/// улучшить до 2-го, а затем и до 3-го уровней. Перед каждым
	/// ударом Вампир получает 1 заряд крови. Если количество
	/// зарядов крови достигает 3 / 2 / 1, то его количество зарядов
	/// крови уменьшается на это число, Вампир восстанавливает
	/// текущее здоровье и увеличивает свой урон на 20% / 25% /
	/// 30% от его урона. (cоответственно уровням 1 / 2 / 3)
	/// Если здоровье героя ниже ноля, а способность поднимает
	/// его выше ноля, герой не умер.
	/// </summary>
	public class Vampire : Hero
	{
		private int _abilityLvl = 1;
		private int _charges = 0; // TODO узнать, переносятся ли заряды в след. бой

		private const int UPGRADE_COST = 100;
		private const int MAX_LVL = 3;

		private readonly int[] _needCharges = { 0, 3, 2, 1 };
		private readonly decimal[] _lifestealLv = { 0, .20m, .25m, .30m };

		private int Lifesteal => (int)(_lifestealLv[_abilityLvl] * StatsRO.Converted<Attack>());
		private int NeedCharges => _needCharges[_abilityLvl];

		public override bool UseAbility()
		{
			if (Money < UPGRADE_COST || _abilityLvl >= MAX_LVL) return false;
			Spend(UPGRADE_COST);
			_abilityLvl++;
			return true;
		}

		public override Damage GetDamageTo(IBattler enemy)
		{
			++_charges;
			if (_charges < NeedCharges) return base.GetDamageTo(enemy);
			_charges = 0;
			Stats.Modify<Health>(Lifesteal); // TODO уточнить, когда происходит отхил
			return base.GetDamageTo(enemy).ModPhys(a => a + Lifesteal);
		}

		public override string AbilityText => $"Уровень {_abilityLvl}, {_charges} зарядов";
	}
}
