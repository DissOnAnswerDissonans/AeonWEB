global using System;
global using Aeon.Base;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AeonCore.Tests")]

namespace Aeon.Core;

public static class RNG
{
	private static Random _random { get; } = new Random((int) DateTime.Now.Ticks);

	public static bool TestChance(double v) => _random.NextDouble() < v;
	public static bool TestChance(decimal v) => TestChance((double) v);
}

public class StatDef
{
	private StatsContainer _context;
	private string _id;
	public StatType StatType => _context[_id].Stat;
	public StatValue StatValue => _context[_id].Value;

	public StatDef(string id, StatsContainer container)
	{
		_id = id;
		_context = container;
	}

	public void Set(StatValue value) => _context.SetValue(_id, value);
	public StatDef Add(StatValue value)
	{
		_context.AddToValue(_id, value);
		return this;
	}
	public StatsContainer.StatContext Edit => _context.EditStat(_id);
	public decimal Converted => _context.ConvertAsIs(_id);

	public static implicit operator StatValue(StatDef def) => def.StatValue;
	public static implicit operator int(StatDef def) => def.StatValue;
	public static implicit operator string(StatDef def) => def._id;
}