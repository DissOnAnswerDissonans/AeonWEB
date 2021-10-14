using System;
using System.Reflection;

namespace AeonCore
{
	abstract public class Stat : ICloneable
	{
		public event EventHandler<int> OnChanged = null;

		public Func<int, double> Convertor { get; protected set; }
		public int MinValue { get; protected set; }
		public int MaxValue { get; protected set; }

		public Type StatID { get; private set; }

		private int _value;
		public int Value {
			get => _value;
			internal set { 
				_value = Math.Clamp(value, MinValue, MaxValue);
				OnChanged?.Invoke(this, _value);
			}
		}

		public double Converted => Convertor(_value);

		protected Stat() { 
			Convertor = a => (double) a;
			MinValue = 0;
			MaxValue = int.MaxValue;
			Init();
		}

		virtual protected void Init() { }

		public object Clone()
		{
			var stat = (Stat) Activator.CreateInstance(StatID);
			stat.StatID = StatID;
			stat._value = _value;
			return stat;
		}

		public static T MakeStat<T>(int value) where T : Stat
		{
			T stat = Activator.CreateInstance<T>();
			stat.StatID = typeof(T);
			stat.Value = value;
			return stat;
		}

		internal Stat Add(Stat stat)
		{
			if (StatID != stat.StatID)
				throw new ArgumentException("", nameof(stat));

			Value += stat.Value;
			return this;
		}


		virtual public void OnBattleStart() { }
		virtual public void AfterHit(Hero hero) { }


	}

	abstract public class DynamicStat : Stat
	{
		public event EventHandler<int> OnDynamicChanged;

		protected virtual int TopLimit => int.MaxValue;
		protected virtual int BotLimit => 0;

		public int DynamicValue { 
			get => _dynValue; 
			protected set {
				_dynValue = Math.Clamp(value, BotLimit, TopLimit);
				OnDynamicChanged?.Invoke(this, _dynValue);
			} 
		}
		protected int _dynValue;

		public double DynConverted => Convertor(DynamicValue);
	}

	/**
<summary>
Здоровье:
	Есть 2 показателя характеристики здоровье: текущее и максимальное.
	Текущее здоровье не может превышать максимальное ни в какой момент игры.
	В Магазине игрок может улучшить только максимальное здоровье героя.
</summary>
*/
	public class Health : DynamicStat {
		protected override int TopLimit => Value;

		public override void OnBattleStart() => DynamicValue = Value;

		public override void AfterHit(Hero hero)
		{
			DynamicValue += hero.StatsRO.ConvInt<Regen>();
		}
	}

	/**
<summary>
Атака:
	Характеристика влияет на скорость уменьшения здоровья оппонента.
</summary>
*/
	public class Attack : Stat { }

	/**
<summary>
Магия:
	Характеристика, как и Атака, влияет на скорость
	уменьшения здоровья оппонента, однако обладает другими
	свойствами.
</summary>
*/
	public class Magic : Stat { }

	/**
<summary>
Критический Шанс:
	Характеристика принимает значения в диапазоне от 0% до
	100%. Показывает вероятность, с которой удар будет
	признан критическим.
</summary>
*/
	public class CritChance : Stat {
		protected override void Init() {
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
		protected override void Init() {
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
	public class Income : DynamicStat {
		int _power;

		public override void OnBattleStart() {
			_power = 0;
			DynamicValue = 0;
		}

		public override void AfterHit(Hero hero)
		{
			++_power;
			DynamicValue = (int) ((Math.Pow(Converted, _power) - 1) * 100);
		}

		protected override void Init() {
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
	public class Block : Stat { }

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
		protected override void Init() {
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
	public class Regen : Stat { }
}
