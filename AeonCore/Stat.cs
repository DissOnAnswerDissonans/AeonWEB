using System;

namespace AeonCore
{
	abstract public class Stat
	{
		public delegate double ConvertorFunc(int value);

		public event EventHandler OnChanged = null;

		public ConvertorFunc Convertor { get; protected init; }
		public int MinValue { get; protected init; }
		public int MaxValue { get; protected init; }

		public virtual string StatName => this.GetType().Name;

		private int _value;
		public int Value {
			get => _value;
			internal set { 
				_value = Math.Clamp(value, MinValue, MaxValue);
				OnChanged?.Invoke(this, new EventArgs()); // UNDONE нужен нормальный ивент
			}
		}

		public double Converted => Convertor(_value);

		protected Stat() { 
			Convertor = a => (double) a;
			MinValue = 0;
			MaxValue = int.MaxValue;
		}
	}

	/**
<summary>
Здоровье:
	Есть 2 показателя характеристики здоровье: текущее и максимальное.
	Текущее здоровье не может превышать максимальное ни в какой момент игры.
	В Магазине игрок может улучшить только максимальное здоровье героя.
</summary>
*/
	public class Health : Stat { 
		public Health() { }
	}

	/**
<summary>
Атака:
	Характеристика влияет на скорость уменьшения здоровья оппонента.
</summary>
*/
	public class Attack : Stat {
		public Attack() { }
	}

	/**
<summary>
Магия:
	Характеристика, как и Атака, влияет на скорость
	уменьшения здоровья оппонента, однако обладает другими
	свойствами.
</summary>
*/
	public class Magic : Stat { 
		public Magic() { }
	}

	/**
<summary>
Критический Шанс:
	Характеристика принимает значения в диапазоне от 0% до
	100%. Показывает вероятность, с которой удар будет
	признан критическим.
</summary>
*/
	public class CritChance : Stat {
		public CritChance() { 
			Convertor = (a) => a / 100.0;
			MaxValue = 100;
		}
	}

	/**
<summary>
Критический Урон:
	Характеристика, как и Критический Шанс, исчисляется в %,
	но принимает любые значения не меньше 0%.
	Показывает отношение урона, нанесенного критическим
	ударом к урону не критического удара.
</summary>
*/
	public class CritDamage : Stat {
		public CritDamage() {
			Convertor = (a) => a / 100.0;
			MinValue = 100;
		}
	}

	/**
<summary>
Прирост:
	Характеристика имеет 2 показателя: текущий прирост и разовый прирост.
	Исчисляется процентами.
	Текущий прирост показывает во сколько раз увеличивается наносимый Урон.
	Разовый прирост показывает на сколько процентов Текущий
	прирост увеличивается после каждого Удара.
	В Магазине можно улучшить только разовый прирост.
</summary>
*/
	public class Multiplier : Stat {
		public Multiplier() {
			Convertor = a => 1 + a / 100.0;
		}
	}

	/**
<summary>
Броня:
	Характеристика показывает, насколько уменьшается получаемый урон.
	В случае если Броня больше Урона, который должен
	получить герой — урон предотвращается.
</summary>
*/
	public class Block : Stat {
		public Block() { }
	}

	/**
<summary>
Щит:
	Характеристика показывает какая часть получаемого урона
	предотвращается. Исчисляется в процентах и принимает
	значения не меньше 0% и не больше 99%.
</summary>
*/
	public class Armor : Stat {
		const double COEFF = 0.0075;
		public Armor() {
			MaxValue = 300;
			Convertor = t => COEFF * t / (1 + COEFF * Math.Exp(0.9 * Math.Log(t)));
		}
	}

	/**
<summary>
Регенерация:
	Характеристика показывает сколько Текущего Здоровья
	герой получит после каждого удара в случае, если останется жив.
	Важным пунктом является, что герой не получает здоровья в
	случае, если его Броня предотвратила получение Урона.
</summary>
*/
	public class Regen : Stat {
		public Regen() { }
	}
}
