﻿using System;
using System.Collections.Generic;

namespace Aeon.Core
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

	public abstract class StatType
	{
		static StatType() => _instances = new Dictionary<Type, StatType>();

		private static readonly Dictionary<Type, StatType> _instances;

		public static T Instance<T>() where T : StatType, new()
		{
			if (_instances.TryGetValue(typeof(T), out StatType stat)) {
				return (T) stat;
			} else {
				var inst = new T();
				_instances[typeof(T)] = inst;
				return inst;
			};
		}

		public string ID { get; protected set; }
		public Func<int, IReadOnlyStats, decimal> Convertor { get; protected set; }
		public int MinValue { get; protected set; }
		public int MaxValue { get; protected set; }

		//public Names DebugNames { get; protected set; }

		protected StatType()
		{
			Convertor = (a, context) => a;
			MinValue = 0;
			MaxValue = int.MaxValue;
			Init();
		}

		protected virtual void Init()
		{
		}
	}

	public abstract class StatTypeDynamic : StatType
	{
		public virtual int TopLimit(IReadOnlyStats stats) => int.MaxValue;

		public virtual int BotLimit(IReadOnlyStats stats) => 0;

		public Func<int, IReadOnlyStats, decimal> DynConvertor { get; protected set; }

		protected StatTypeDynamic()
		{
			DynConvertor = (a, context) => a;
			InitDyn();
		}

		protected virtual void InitDyn()
		{
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

	public class Health : StatTypeDynamic
	{
		public override int TopLimit(IReadOnlyStats stats) => stats.ConvInt<Health>();

		protected override void Init() => ID = "HP";
	}

	/**
<summary>
Атака:
	Характеристика влияет на скорость уменьшения здоровья оппонента.
</summary>
*/

	public class Attack : StatType
	{
		protected override void Init() => ID = "ATT";
	}

	/**
<summary>
Магия:
	Характеристика, как и Атака, влияет на скорость
	уменьшения здоровья оппонента, однако обладает другими
	свойствами.
</summary>
*/

	public class Magic : StatType
	{
		protected override void Init() => ID = "MAG";
	}

	/**
<summary>
Критический Шанс:
	Характеристика принимает значения в диапазоне от 0% до
	100%. Показывает вероятность, с которой удар будет
	признан критическим.
</summary>
*/

	public class CritChance : StatType
	{
		protected override void Init()
		{
			ID = "CHA";
			Convertor = (a, context) => a / 100.0m;
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

	public class CritDamage : StatType
	{
		protected override void Init()
		{
			ID = "CDM";
			Convertor = (a, context) => a / 100.0m;
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

	public class Income : StatTypeDynamic
	{
		protected override void Init()
		{
			ID = "INC";
			Convertor = (a, context) => 1 + a / 100.0m;
		}

		protected override void InitDyn()
		{
			DynConvertor = (a, context) => {
				decimal d = 1;
				decimal f = context.Converted<Income>();
				for (int i = 0; i < a; ++i)
					d *= f;
				return d;
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

	public class Block : StatType
	{
		protected override void Init() => ID = "BLK";
	}

	/**
<summary>
Щит:
	Характеристика показывает какая часть получаемого урона
	предотвращается. Исчисляется в процентах и принимает
	значения не меньше 0% и не больше 99%.
</summary>
*/

	public class Armor : StatType
	{
		private const decimal COEFF = 0.0075m;

		protected override void Init()
		{
			ID = "ARM";
			MaxValue = 300;
			Convertor = (t, context) => COEFF * t / (1 + COEFF * (decimal) Math.Exp(0.9 * Math.Log(t)));
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
		protected override void Init() => ID = "REG";
	}
}