namespace Aeon.Core
{
	public interface IReadOnlyStats
	{
		public Stat this[StatType type] { get; }

		public Stat Get<TStat>() where TStat : StatType, new();

		public int RawValue<TStat>() where TStat : StatType, new() => 
			Get<TStat>().Value;

		public decimal Converted<TStat>() where TStat : StatType, new() => 
			Get<TStat>().Converted;

		public int ConvInt<TStat>() where TStat : StatType, new() => 
			(int) Converted<TStat>();


		public DynStat GetDyn<TStat>() where TStat : StatTypeDynamic, new();

		public int DynamicValue<TStat>() where TStat : StatTypeDynamic, new() =>
			GetDyn<TStat>().Value;

		public decimal DynConverted<TStat>() where TStat : StatTypeDynamic, new() =>
			GetDyn<TStat>().Converted;

		public int DynConvInt<TStat>() where TStat : StatTypeDynamic, new() =>
			(int) DynConverted<TStat>();
	}
}
