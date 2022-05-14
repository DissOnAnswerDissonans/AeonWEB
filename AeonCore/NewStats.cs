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
	public int DynConvert(string id) => TryConvertDyn(id).Value.TRound();
	public decimal DynConvertAsIs(string id) => TryConvertDyn(id).Value;

	public StatValue? TryGetValue(string id);
	public StatValue? TryGetDynValue(string id);
	public decimal? TryConvert(string id);
	public decimal? TryConvertDyn(string id);

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
			_types.Add(id, t);
			_values.Add(id, new StatValue());
			return new StatContext(this, _types[id], _values[id]);
		}
		catch (ArgumentException) {
			//System.Diagnostics.Debug.Fail($"Stat {id} duplicate");
			return null;
		}
	}

	public StatContext EditStat(string id) => new(this, _types[id], _values[id]);

	public int GetValue(string id) => TryGetValue(id)?.Value ?? throw new KeyNotFoundException();
	public int GetDynValue(string id) => TryGetDynValue(id)?.Value ?? throw new KeyNotFoundException();
	public int Convert(string id) => TryConvert(id)?.TRound() ?? throw new KeyNotFoundException();
	public decimal ConvertAsIs(string id) => TryConvert(id) ?? throw new KeyNotFoundException();

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

	private StatValue SetLimitted(string id, StatValue value) => _values[id] = _types[id].Limits(value, this);
	public bool SetValue(string id, StatValue value)
	{		
		if (_values.ContainsKey(id)) {
			SetLimitted(id, value);
			StatChanged?.Invoke(_types[id], value);
			foreach (var cx in _types[id].DependentIDs)
				SetLimitted(cx, _values[cx]);
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
			StatValue value = SetLimitted(id, GetValue(id) + amount);
			StatChanged?.Invoke(_types[id], value);
			return value;
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
	public decimal? TryConvertDyn(string id) => TryConvert(Dyn(id));

	public void Reset(params string[] ids) => ids.ToList().ForEach(id => _values[id] = _types[id].Default(this));

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
			StatType s = Stat with { Limits = limitter };
			Context._types[Stat.ID] = s;
			StatValue v = Context.SetLimitted(Stat.ID, Value);
			return this with { Stat = s, Value = v };
		}
		public StatContext Limit(Func<int, int> func) => Limit((x, ctx) => func(x));
		public StatContext Limit(int min, int max) => Limit(GetLimitter(min, max));
		public StatContext Limit(int max) => Limit(GetLimitter(max));

		public StatContext Convert(Conv func)
		{
			StatType s = Stat with { Converter = func };
			Context._types[Stat.ID] = s;
			return this with { Stat = s };
		}
		public StatContext Convert(Func<int, decimal> func) => Convert((x, ctx) => func(x));

		public StatContext AddDynamic() => AddDynamic(x => 0);
		public StatContext AddDynamic(bool setMax) => setMax? AddDynamic(x => x) : AddDynamic();
		public StatContext AddDynamic(Func<int, int> @default)
		{
			string id = Stat.ID;
			return Context.UncheckedNewStat(Dyn(id))?.Dependent(id).Default(ctx => @default(ctx.Convert(id)));
		}

		public StatContext Default(ContextValue<int> defaultGetter)
		{
			StatType s = Stat with { Default = defaultGetter };
			Context._types[Stat.ID] = s;
			if (Value.Value == 0) Context.Reset(Stat.ID);
			return this with { Stat = s, Value = Context._values[Stat.ID] };
		}
		public StatContext Default(int value) => Default(x => value);

		public StatContext Dependent(params string[] ids)
		{
			ids.ToList().ForEach(id => Context._types[id].DependentIDs.Add(Stat.ID)); // осторожно, ГРЯЗЬ
			return this;
		}
	}
}

public record class StatType
{
	public delegate decimal Conv(int i, IStatContext ctx);
	public delegate int Limitter(int i, IStatContext ctx);
	public delegate T ContextValue<T>(IStatContext ctx);

	public string ID { get; }
	public Conv Converter { get; init; } = (x, ctx) => x;
	public Limitter Limits { get; init; } = (x, ctx) => Math.Max(0, x);
	public ContextValue<int> Default { get; init; } = ctx => 0;
	public HashSet<string> DependentIDs { get; } = new();

	internal StatType(string id) => ID = id;
	internal StatType(string id, Conv func) : this(id) => Converter = func;
	internal static Limitter GetLimitter(int val) => (x, ctx) => Math.Clamp(x, 0, val);
	internal static Limitter GetLimitter(int min, int max) => (x, ctx) => Math.Clamp(x, min, max);
}

public struct StatValue
{
	public int Value { get; init; }

	public static implicit operator int(StatValue value) => value.Value;
	public static implicit operator StatValue(int value) => new() { Value = value };
}