using System;

namespace Aeon.Core;

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

	public Damage ModPhys(Func<int, int> newPhys) =>
		new(Instigator, newPhys(Phys), Magic, IsCrit);

	public Damage ModMag(Func<int, int> newMag) =>
		new(Instigator, Phys, newMag(Magic), IsCrit);

	public override string ToString() => $"{Phys}{(IsCrit? "!" : "")}[P] + {Magic}[M]";
}
