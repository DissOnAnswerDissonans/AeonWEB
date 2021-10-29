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

		private const int MONEY_BURN = 2;
		private const int MONEY_BURN_COST = 3;

		private int MagHitAdder => (int) (StatsRO.Converted<Magic>() * MAG_HIT_BONUS);
		private const decimal MAG_HIT_BONUS = 0.3m;
		private const int MAG_HIT_COST = 4;

		private const decimal HEALING_COEFF = 0.2m;
		private const int HEALING_COST = 5;

		public override bool UseAbility()
		{
			_mode = (Mode) (((int) _mode + 1) % 4);
			return true;
		}

		public override string AbilityText => _mode switch {
			Mode.AbilityOff => $"M{_mana,-2}| Накопление",
			Mode.MoneyBurn  => $"M{_mana,-2}| Сжигание ${MONEY_BURN} ~ {MONEY_BURN_COST}м",
			Mode.MagicHit   => $"M{_mana,-2}| +{MAG_HIT_BONUS:P0} ({MagHitAdder}) МАГ ~ {MAG_HIT_COST}м",
			Mode.Healing    => $"M{_mana,-2}| +{HEALING_COEFF:P0} сбитого ЗДР ~ {HEALING_COST}м",
		};

		public override Damage GetDamageTo(IBattler enemy)
		{
			Damage d = base.GetDamageTo(enemy);
			++_mana;

			switch (_mode) {

			case Mode.AbilityOff:
				return d;

			case Mode.MoneyBurn:
				if (_mana < MONEY_BURN_COST) return d;
				_mana -= MONEY_BURN_COST;
				(enemy as Hero).Spend(MONEY_BURN);
				return d;

			case Mode.MagicHit:
				if (_mana < MAG_HIT_COST) return d;
				_mana -= MAG_HIT_COST;
				return d.ModMag(a => a + MagHitAdder);

			case Mode.Healing:
				if (_mana < HEALING_COST) return d;
				_mana -= HEALING_COST;
				Stats.ModifyDyn<Health>
					((int)((1 - StatsRO.DynConvInt<Health>() / StatsRO.ConvInt<Health>()) * HEALING_COEFF));
				return d;

			default: return d;
			}
		}
	}
}
