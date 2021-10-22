namespace AeonCore
{
	public interface IReadOnlyStats
	{
		public Stat Get<TStat>() where TStat : StatType, new();

		public int RawValue<TStat>() where TStat : StatType, new() => 
			Get<TStat>().Value;

		public double Converted<TStat>() where TStat : StatType, new() => 
			Get<TStat>().Converted;

		public int ConvInt<TStat>() where TStat : StatType, new() => 
			(int) Converted<TStat>();

		//public int DynamicValue<TStat>() where TStat : DynamicStat => 
		//	Get<TStat>().DynamicValue;

		//public double DynConverted<TStat>() where TStat : DynamicStat =>
		//	Get<TStat>().DynConverted;

		//public int DynConvInt<TStat>() where TStat : DynamicStat =>
		//	(int) DynConverted<TStat>();
	}
}
