namespace Aeon.Core
{
	public struct Damage
	{
		public IBattler Instigator { get; init; }
		public int Phys { get; init; }
		public int Magic { get; init; }
		public bool IsCrit { get; init; }
	}
}
