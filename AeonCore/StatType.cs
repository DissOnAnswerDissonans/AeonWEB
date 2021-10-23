using System;
using System.Collections.Generic;
using System.Reflection;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AeonCore.Tests")]
namespace AeonCore
{
	/// <summary>
	/// 
	/// Базовый Класс поведений статов.
	/// Для создания своего стата надо наследоваться от него 
	/// и переопределить свойства? .. 
	/// 
	/// Тут надо подумать насчет записи, по идее, это должен
	/// быть наследуемый одиночка (1 экз. на каждый подкласс)
	/// 
	/// </summary>

	abstract public class StatType
	{
		public struct Names
		{
			public string FullNameEN { get; init; }
			public string FullNameRU { get; init; }
			public string AliasEN { get; init; }
			public string AliasRU { get; init; }
		}

		static StatType()
		{
			_instances = new Dictionary<Type, StatType>();
		}

		static Dictionary<Type, StatType> _instances;
		internal static T Instance<T>() where T : StatType, new()
		{
			if (_instances.TryGetValue(typeof(T), out StatType stat))
				return (T) stat;
			else {
				var inst = new T();
				_instances[typeof(T)] = inst;
				return inst;
			};
		}


		public Func<int, double> Convertor { get; protected set; }
		public int MinValue { get; protected set; }
		public int MaxValue { get; protected set; }

		public Names DebugNames { get; protected set; }

		protected StatType() { 
			Convertor = a => (double) a;
			MinValue = 0;
			MaxValue = int.MaxValue;
			Init();
		}

		virtual protected void Init() { }
	}


	abstract public class StatTypeDynamic : StatType
	{
		public virtual int TopLimit(IReadOnlyStats stats) => int.MaxValue;
		public virtual int BotLimit(IReadOnlyStats stats) => 0;


		//virtual public bool OnBattleStart(ref DynStat dynStat, in Hero hero, in Hero enemy) => false;
		//virtual public bool AfterHit(ref DynStat dynStat, in Hero hero, in Damage damage) => false;

		//public int DynamicValue { 
		//	get => _dynValue; 
		//	protected set {
		//		_dynValue = Math.Clamp(value, BotLimit, TopLimit);
		//		OnDynamicChanged?.Invoke(this, _dynValue);
		//	} 
		//}
		//protected int _dynValue;

		//public double DynConverted => Convertor(DynamicValue);
	}

	/**
<summary>
Здоровье:
	Есть 2 показателя характеристики здоровье: текущее и максимальное.
	Текущее здоровье не может превышать максимальное ни в какой момент игры.
	В Магазине игрок может улучшить только максимальное здоровье героя.
</summary>
*/
	public class Health : StatTypeDynamic {
		public override int TopLimit(IReadOnlyStats stats) => stats.ConvInt<Health>();

		protected override void Init()
		{
			DebugNames = new Names() {
				FullNameEN = "Health",
				FullNameRU = "Здоровье",
				AliasEN = "HP",
				AliasRU = "ЗДР",
			};		
		}

		//public override bool OnBattleStart(ref DynStat dynStat, in Hero hero, in Hero enemy)
		//{
		//	dynStat.Value = hero.StatsRO.ConvInt<Health>();
		//	return true;
		//}

		//public override bool AfterHit(ref DynStat dynStat, in Hero hero, in Damage damage)
		//{
		//	if (damage.Phys > 0)
		//		dynStat.Value += hero.StatsRO.ConvInt<Regen>();
		//	return true;
		//}
	}

	/**
<summary>
Атака:
	Характеристика влияет на скорость уменьшения здоровья оппонента.
</summary>
*/
	public class Attack : StatType {
		protected override void Init()
		{
			DebugNames = new Names() {
				FullNameEN = "Attack",
				FullNameRU = "Атака",
				AliasEN = "ATT",
				AliasRU = "АТК",
			};
		}
	}

	/**
<summary>
Магия:
	Характеристика, как и Атака, влияет на скорость
	уменьшения здоровья оппонента, однако обладает другими
	свойствами.
</summary>
*/
	public class Magic : StatType {
		protected override void Init()
		{
			DebugNames = new Names() {
				FullNameEN = "Magic",
				FullNameRU = "Магия",
				AliasEN = "MAG",
				AliasRU = "МАГ",
			};
		}
	}

	/**
<summary>
Критический Шанс:
	Характеристика принимает значения в диапазоне от 0% до
	100%. Показывает вероятность, с которой удар будет
	признан критическим.
</summary>
*/
	public class CritChance : StatType {
		protected override void Init() {
			Convertor = (a) => a / 100.0;
			MaxValue = 100;
			DebugNames = new Names() {
				FullNameEN = "Critical Chance",
				FullNameRU = "Критический Шанс",
				AliasEN = "CRC",
				AliasRU = "КША",
			};
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
	public class CritDamage : StatType {
		protected override void Init() {
			Convertor = (a) => a / 100.0;
			MinValue = 100;
			DebugNames = new Names() {
				FullNameEN = "Critical Attack",
				FullNameRU = "Критический Урон",
				AliasEN = "CAT",
				AliasRU = "КУР",
			};
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
	public class Income : StatTypeDynamic {

		//public override bool OnBattleStart(ref DynStat dynStat, in Hero hero, in Hero enemy) 
		//{
		//	dynStat.Value = 0;
		//	return true;
		//}

		//public override bool AfterHit(ref DynStat dynStat, in Hero hero, in Damage damage)
		//{
		//	var v = hero.StatsRO.Converted<Income>();
		//	dynStat.Value = (int) ((100 + dynStat.Value) * v - 100);
		//	return true;
		//}

		protected override void Init() {
			Convertor = a => 1 + a / 100.0;
			DebugNames = new Names() {
				FullNameEN = "Income",
				FullNameRU = "Прирост",
				AliasEN = "INC",
				AliasRU = "ПРС",
			};
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
	public class Block : StatType {
		protected override void Init()
		{
			DebugNames = new Names() {
				FullNameEN = "Block",
				FullNameRU = "Броня",
				AliasEN = "BLK",
				AliasRU = "БРН",
			};
		}
	}

	/**
<summary>
Щит:
	Характеристика показывает какая часть получаемого урона
	предотвращается. Исчисляется в процентах и принимает
	значения не меньше 0% и не больше 99%.
</summary>
*/
	public class Armor : StatType {
		const double COEFF = 0.0075;
		protected override void Init() {
			MaxValue = 300;
			Convertor = t => COEFF * t / (1 + COEFF * Math.Exp(0.9 * Math.Log(t)));
			DebugNames = new Names() {
				FullNameEN = "Armor",
				FullNameRU = "Защита",
				AliasEN = "ARM",
				AliasRU = "ЩИТ",
			};
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
	public class Regen : StatType
	{
		protected override void Init()
		{
			DebugNames = new Names() { 
				FullNameEN = "Regeneration", 
				FullNameRU = "Регенерация", 
				AliasEN = "REG", 
				AliasRU = "РЕГ" 
			};
		}
	}
}
