using System;

namespace Aeon.Core
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

		public decimal Convert(IReadOnlyStats context) => Behaviour.Convertor(Value, context);

		public static Stat Make<T>(int value) where T : StatType, new()
		{
			return new Stat {
				Behaviour = StatType.Instance<T>(),
				Value = value,
			};
		}

		internal Stat Add(Stat stat)
		{
			if (Behaviour.ID != stat.Behaviour.ID)
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
			return Value = Math.Clamp(value, Behaviour.BotLimit(context), Behaviour.TopLimit(context));
			//OnChanged?.Invoke(this, _value);
		}



	public static DynStat Make<T>(int value) where T : StatTypeDynamic, new()
		{
			return new DynStat {
				Behaviour = StatType.Instance<T>(),
				Value = value,
			};
		}

		public decimal Convert(IReadOnlyStats context) => Behaviour.DynConvertor(Value, context);

	}
}
