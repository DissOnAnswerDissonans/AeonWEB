﻿using System;

namespace AeonCore
{
	//interface IStat
	//{
	//	StatType StatType { get; }
	//	int Value { get; }
	//}

	public struct Stat //: IStat
	{
		internal StatType Behaviour { get; init; }
		public StatType StatType => Behaviour;

		private int _value;
		public int Value {
			get => _value;
			internal set {
				_value = Math.Clamp(value, Behaviour.MinValue, Behaviour.MaxValue);
				//OnChanged?.Invoke(this, _value);
			}
		}

		public double Converted => Behaviour.Convertor(_value);

		public static Stat Make<T>(int value) where T : StatType, new()
		{
			return new Stat {
				Behaviour = StatType.Instance<T>(),
				Value = value,
			};
		}

		internal Stat Add(Stat stat)
		{
			if (Behaviour != stat.Behaviour)
				throw new ArgumentException("", nameof(stat));

			return new Stat {
				Behaviour = this.Behaviour,
				Value = this.Value + stat.Value,
			};
		}

		public static Stat operator +(Stat s1, Stat s2) => s1.Add(s2);
	}

	public struct DynStat //: IStat
	{
		internal StatTypeDynamic Behaviour { get; init; }
		public StatType StatType => Behaviour;

		public int Value { get; private set; }

		internal int SetValue(int value, IReadOnlyStats context) {
			return Value = Math.Clamp(value, Behaviour.TopLimit(context), Behaviour.BotLimit(context));
			//OnChanged?.Invoke(this, _value);
		}



	public static DynStat Make<T>(int value) where T : StatTypeDynamic, new()
		{
			return new DynStat {
				Behaviour = StatType.Instance<T>(),
				Value = value,
			};
		}

		public double Converted => Behaviour.Convertor(Value);

	}
}
