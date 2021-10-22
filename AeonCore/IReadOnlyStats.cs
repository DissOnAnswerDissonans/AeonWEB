namespace AeonCore
{
	public interface IReadOnlyStats
	{
		public TStat Get<TStat>() where TStat : StatBehaviour;

		public int RawValue<TStat>() where TStat : StatBehaviour => 
			Get<TStat>().Value;

		public double Converted<TStat>() where TStat : StatBehaviour => 
			Get<TStat>().Converted;

		public int ConvInt<TStat>() where TStat : StatBehaviour => 
			(int) Converted<TStat>();

		public int DynamicValue<TStat>() where TStat : DynamicStat => 
			Get<TStat>().DynamicValue;

		public double DynConverted<TStat>() where TStat : DynamicStat =>
			Get<TStat>().DynConverted;

		public int DynConvInt<TStat>() where TStat : DynamicStat =>
			(int) DynConverted<TStat>();
	}
}
