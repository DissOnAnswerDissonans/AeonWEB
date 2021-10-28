using System;

namespace Aeon.Core
{
	public struct Damage
	{
		public IBattler Instigator { get; init; }
		public int Phys { get; init; }
		public int Magic { get; init; }
		public bool IsCrit { get; init; }

		public Damage(IBattler inst, int phys, int mag, bool crit)
		{
			Instigator = inst;
			Phys = phys;
			Magic = mag;
			IsCrit = crit;
		}

		public Damage ModPhys(Func<int, int> newPhys)
		{
			return new Damage(Instigator, newPhys(Phys), Magic, IsCrit);
		}

		public Damage ModMag(Func<int, int> newMag)
		{
			return new Damage(Instigator, Phys, newMag(Magic), IsCrit);
		}
	}
}
