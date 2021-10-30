namespace Aeon.Core
{
	public interface IReadOnlyStats
	{
		public Stat this[StatType type] { get; }

		public Stat GetStat<TStat>() where TStat : StatType, new();

		public int RawValue<TStat>() where TStat : StatType, new() =>
			GetStat<TStat>().Value;

		public decimal Converted<TStat>() where TStat : StatType, new() =>
			GetStat<TStat>().Convert(this);

		public int ConvInt<TStat>() where TStat : StatType, new() =>
			(int) Converted<TStat>();

		//

		public DynStat GetDyn<TStat>() where TStat : StatTypeDynamic, new();

		public int DynamicValue<TStat>() where TStat : StatTypeDynamic, new() =>
			GetDyn<TStat>().Value;

		public decimal DynConverted<TStat>() where TStat : StatTypeDynamic, new() =>
			GetDyn<TStat>().Convert(this);

		public int DynConvInt<TStat>() where TStat : StatTypeDynamic, new() =>
			(int) DynConverted<TStat>();
	}
}