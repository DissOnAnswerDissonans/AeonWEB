using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Универсальный герой. Обладает дополнительной
	/// характеристикой — маной. Также имеет 4 режима своей
	/// способности:
	/// 1. Следующий удар уменьшает количество игровой валюты
	/// героя противника на 2 (стоит 3 маны)
	/// 2. На время следующего удара магия Кровавого Эльфа
	/// увеличивается на 30% (стоит 4 маны)
	/// 3. После следующего удара Кровавый Эльф восстанавливает
	/// 20% недостающего текущего здоровья(стоит 5 маны)
	/// 0. Мана не тратится
	/// Перед каждой атакой количество маны увеличивается на 1,
	/// и, если маны хватает на применение текущей способности —
	/// ее стоимость вычитается из маны, и эта способность
	/// применяется.Режим способности можно свободно менять в
	/// Магазине.
	/// </summary>
	public class BloodyElf : Hero
	{
		private enum Mode
		{
			AbilityOff,
			MoneyBurn,
			MagicHit,
			Healing,
		}

		private int _mana;
		private Mode _mode;

		private static int moneyBurn = 2;
		private static int moneyBurnCost = 3;

		private static decimal magHitBonus = 0.3m;
		private static int magHitCost = 4;

		private static decimal healingCoeff = 0.2m;
		private static int healingCost = 5;

		private int MagHitAdder => (int) (StatsRO.Converted<Magic>() * magHitBonus);


		public override bool UseAbility()
		{
			_mode = (Mode) (((int) _mode + 1) % 4);
			return true;
		}

		public override string AbilityText => _mode switch {
			Mode.AbilityOff => $"M{_mana,-2}| Накопление",
			Mode.MoneyBurn => $"M{_mana,-2}| Сжигание ${moneyBurn} ~ {moneyBurnCost}м",
			Mode.MagicHit => $"M{_mana,-2}| +{magHitBonus:P0} ({MagHitAdder}) МАГ ~ {magHitCost}м",
			Mode.Healing => $"M{_mana,-2}| +{healingCoeff:P0} сбитого ЗДР ~ {healingCost}м",
		};

		public override Damage GetDamageTo(IBattler enemy)
		{
			Damage d = base.GetDamageTo(enemy);
			++_mana;

			switch (_mode) {
			case Mode.AbilityOff:
				return d;

			case Mode.MoneyBurn:
				if (_mana < moneyBurnCost) return d;
				_mana -= moneyBurnCost;
				(enemy as Hero).Spend(moneyBurn);
				return d;

			case Mode.MagicHit:
				if (_mana < magHitCost) return d;
				_mana -= magHitCost;
				return d.ModMag(a => a + MagHitAdder);

			case Mode.Healing:
				if (_mana < healingCost) return d;
				_mana -= healingCost;
				Stats.ModifyDyn<Health>
					((int) ((1 - StatsRO.DynConvInt<Health>() / StatsRO.ConvInt<Health>()) * healingCoeff));
				return d;

			default: return d;
			}
		}
	}
}