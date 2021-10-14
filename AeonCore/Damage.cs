namespace AeonCore
{
	public struct Damage
	{
		public Hero Instigator { get; init; }
		public int Phys { get; init; }
		public int Magic { get; init; }
		public bool IsCrit { get; init; }
	}
}
