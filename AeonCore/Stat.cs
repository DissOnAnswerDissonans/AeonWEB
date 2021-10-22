using System;

namespace AeonCore
{
	public struct Stat
	{
		internal StatBehaviour Behaviour { get; init; }

		private int _value;
		public int Value {
			get => _value;
			internal set {
				_value = Math.Clamp(value, Behaviour.MinValue, Behaviour.MaxValue);
				//OnChanged?.Invoke(this, _value);
			}
		}

		public double Converted => Behaviour.Convertor(_value);


		public static Stat MakeStat<T>(int value) where T : StatBehaviour, new()
		{
			return new Stat { 
				Behaviour = StatBehaviour.Instance<T>(), 
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
}
