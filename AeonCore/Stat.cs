using System;

namespace AeonCore
{
	public struct Stat
	{
		internal StatType Behaviour { get; init; }

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

	public struct DynStat
	{
		internal StatTypeDynamic Behaviour { get; init; }

		private int _value;
		public int Value {
			get => _value;
			internal set {
				_value = Math.Clamp(value, Behaviour.TopLimit, Behaviour.BotLimit);
				//OnChanged?.Invoke(this, _value);
			}
		}


	}
}
