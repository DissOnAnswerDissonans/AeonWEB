using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Aeon.Core.StatType;

namespace Aeon.Core;



public interface IStatContext
{
	public event Action<StatType, StatValue> StatChanged;
	public (StatType Stat, StatValue Value) this[string id] { get; }

	public int GetValue(string id) => TryGetValue(id).Value.Value;
	public int GetDynValue(string id) => TryGetDynValue(id).Value.Value;
	public int Convert(string id) => TryConvert(id).Value.TRound();
	public decimal ConvertAsIs(string id) => TryConvert(id).Value;

	public StatValue? TryGetValue(string id);
	public StatValue? TryGetDynValue(string id);
	public decimal? TryConvert(string id);

	public Base.StatData GetBase(string id) => new() {
		StatId = id, RawValue = TryGetValue(id).Value, Value = TryConvert(id),
	};
}

public class StatsContainer : IStatContext
{
	private Dictionary<string, StatType> _types = new();
	private Dictionary<string, StatValue> _values = new();

	public event Action<StatType, StatValue> StatChanged;

	static string Dyn(string s) => $"<DYN>{s}";

	public StatContext NewStat(string id) => Checks(id) ? UncheckedNewStat(id) : null;

	private static bool Checks(string id)
	{
		if (string.IsNullOrEmpty(id)) return false;
		if (id.Contains('<') || id.Contains('>')) return false;
		return true;
	}

	private StatContext UncheckedNewStat(string id)
	{
		try {
			StatType t = new StatType(id);
			return new StatContext(this, _types[id], _values[id]);
		}
		catch (ArgumentException) {
			//System.Diagnostics.Debug.Fail($"Stat {id} duplicate");
			return null;
		}
	}

	public StatContext EditStat(string id) => new(this, _types[id], _values[id]);

	public int GetValue(string id) => TryGetValue(id).Value.Value;
	public int GetDynValue(string id) => TryGetDynValue(id).Value.Value;
	public int Convert(string id) => TryConvert(id).Value.TRound();
	public decimal ConvertAsIs(string id) => TryConvert(id).Value;

	public StatValue? TryGetValue(string id)
	{
		try {
			return _values[id];
		} catch (KeyNotFoundException) {
			//System.Diagnostics.Debug.Fail($"Stat {id} not found");
			return null;
		}
	}
	public StatValue? TryGetDynValue(string id) => TryGetValue(Dyn(id));

	public bool SetValue(string id, StatValue value)
	{		
		if (_values.ContainsKey(id)) {
			_values[id] = value;
			StatChanged?.Invoke(_types[id], value);
			return true;
		} else {
			//System.Diagnostics.Debug.Fail($"Stat {id} not found, call NewStat() before");
			return false;
		}
	}
	public bool SetDynValue(string id, StatValue value) => SetValue(Dyn(id), value);

	public StatValue? AddToValue(string id, StatValue amount)
	{
		try {
			var x = _values[id] += amount;
			StatChanged?.Invoke(_types[id], x);
			return x;
		} catch (KeyNotFoundException) {
			return null;
		}
	}
	public StatValue? AddToDynValue(string id, StatValue amount) => AddToValue(Dyn(id), amount);

	public decimal? TryConvert(string id)
	{
		var value = TryGetValue(id);
		return value switch {
			null => null,
			_ => _types[id].Converter(value!.Value, this)
		};
	}

	private void Reset(Func<string, bool> predicate) => 
		_types.Keys.Where(predicate).ToList().ForEach(x => _values[x] = _types[x].Default(this));

	public void ResetAll() => Reset(x => true);
	public void ResetDynamic() => Reset(x => x.StartsWith(Dyn("")));


	public (StatType Stat, StatValue Value) this[string id] => (_types[id], _values[id]);
	public IEnumerable<(StatType Stat, StatValue Value)> All()
	{
		foreach (var id in _types.Keys)
			yield return (_types[id], _values[id]);
	}

	public record StatContext(StatsContainer Context, StatType Stat, StatValue Value)
	{
		public StatContext Set(StatValue value)
		{
			Context.SetValue(Stat.ID, value);
			return this with { Value = Context.TryGetValue(Stat.ID).Value };
		}

		public StatContext Limit(Limitter limitter)
		{
			Context._types[Stat.ID] = Stat with { Limits = limitter };
			return this;
		}
		public StatContext Limit(Limits limits) => Limit(GetLimitter(limits));
		public StatContext Limit(int max) => Limit(new Limits(max));

		public StatContext Convert(Conv func)
		{
			Context._types[Stat.ID] = Stat with { Converter = func };
			return this;
		}

		public StatContext AddDynamic() => Context.UncheckedNewStat(Dyn(Stat.ID));
		public StatContext AddDynamicDefault() => AddDynamic().Default(x => (int) x.TryConvert(Stat.ID));

		public StatContext Default(ContextValue<int> defaultGetter)
		{
			Context._types[Stat.ID] = Stat with { Default = defaultGetter };
			return this;
		}
		public StatContext Default(int value) => Default(x => value);
	}
}

public record class StatType
{
	public delegate decimal Conv(int i, IStatContext ctx);
	public delegate int Limitter(int i, IStatContext ctx);
	public delegate T ContextValue<T>(IStatContext ctx);

	public string ID { get; }
	public Conv Converter { get; init; } = (x, ctx) => x;
	public Limitter Limits { get; init; } = (x, ctx) => x;
	public ContextValue<int> Default { get; init; } = ctx => 0;

	internal StatType(string id) => ID = id;
	internal StatType(string id, Conv func) : this(id) => Converter = func;
	internal static Limitter GetLimitter(Limits limits) => (x, ctx) => Math.Clamp(x, limits.MinValue, limits.MaxValue);
}

public struct StatValue
{
	public int Value { get; init; }

	public static implicit operator int(StatValue value) => value.Value;
	public static implicit operator StatValue(int value) => new() { Value = value };
}

public struct Limits // не факт, что нужен
{
	public Limits() : this(0, 0, int.MaxValue) { }
	public Limits(int maxValue) : this(0, 0, maxValue) { }
	public Limits(int minValue, int maxValue) : this(minValue, minValue, maxValue) { }
	public Limits(int minValue, int defaultValue, int maxValue)
	{
		MinValue = minValue;
		DefaultValue = defaultValue;
		MaxValue = maxValue;
	}

	public int MinValue { get; } = 0;
	public int DefaultValue { get; } = 0;
	public int MaxValue { get; } = int.MaxValue;
}