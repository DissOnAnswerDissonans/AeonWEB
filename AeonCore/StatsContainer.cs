using System;
using System.Collections.Generic;
using System.Linq;

namespace Aeon.Core
{
	public class StatsContainer : IReadOnlyStats
	{
		private readonly Dictionary<StatType, Stat> _stats = new();
		private readonly Dictionary<StatTypeDynamic, DynStat> _dynStats = new();
		private readonly Dictionary<StatType, StatType> _overrides = new();

		public IReadOnlyList<Stat> AllStats => _stats.Values.ToList();

		public Stat this[StatType type] => _stats[type];

		public bool Register<TStat>(int value) where TStat : StatType, new()
		{
			var stat = Stat.Make<TStat>(value);
			try {
				_stats.Add(stat.Behaviour, stat);
			}
			catch (Exception e) {
				throw new InvalidOperationException(
					$"Stat {stat.Behaviour} is already registered", e);
			}
			return true;
		}

		public bool RegisterDyn<TStat>(int value) where TStat : StatTypeDynamic, new()
		{
			if (Register<TStat>(value)) {
				var dyn = DynStat.Make<TStat>(0);
				_dynStats.Add(dyn.Behaviour, dyn);
				return true;
			}
			return false;
		}

		public void Set<TStat>(int value) where TStat : StatType, new()
		{
			if (!_stats.ContainsKey(StatType.Instance<TStat>())) {
				throw new InvalidOperationException(
					$"Stat {typeof(TStat).Name} is not registered");
			}
			_stats[StatType.Instance<TStat>()] = Stat.Make<TStat>(value);
		}

		public Stat GetStat<TStat>() where TStat : StatType, new()
		{
			try {
				return _stats[StatType.Instance<TStat>()];
			}
			catch (Exception e) {
				throw new InvalidOperationException(
					$"Stat {typeof(TStat).Name} is not registered", e);
			}
		}

		public void SetDyn<TStat>(int value) where TStat : StatTypeDynamic, new()
		{
			try {
				_dynStats[StatType.Instance<TStat>()] = DynStat.Make<TStat>(value);
			}
			catch (Exception e) {
				throw new InvalidOperationException(
					$"Stat {typeof(TStat).Name} is not registered for dyn", e);
			}
		}

		public DynStat GetDyn<TStat>() where TStat : StatTypeDynamic, new()
		{
			try {
				return _dynStats[StatType.Instance<TStat>()];
			}
			catch (Exception e) {
				throw new InvalidOperationException(
					$"Stat {typeof(TStat).Name} is not registered for dyn", e);
			}
		}

		public void AddStat(Stat stat)
		{
			try {
				_stats[stat.Behaviour] += stat;
			}
			catch (KeyNotFoundException) {
				throw new InvalidOperationException(
					$"Stat {stat.StatType.GetType()} is not registered");
			}
		}

		public Stat Modify<TStat>(int delta) where TStat : StatType, new()
		{
			Stat stat = GetStat<TStat>();
			stat.Value += delta;
			return _stats[StatType.Instance<TStat>()] = stat;
		}

		public DynStat ModifyDyn<TStat>(int delta) where TStat : StatTypeDynamic, new()
		{
			DynStat stat = GetDyn<TStat>();
			stat.SetValue(stat.Value + delta, this);
			return _dynStats[StatType.Instance<TStat>()] = stat;
		}

		public void OverrideBehaviour<TStat, Tnew>()
			where TStat : StatType, new() where Tnew : TStat, new()
		{
			try {
				_stats[StatType.Instance<TStat>()] = Stat.Make<Tnew>(GetStat<TStat>().Value);
				_overrides.Add(StatType.Instance<TStat>(), StatType.Instance<Tnew>());
			}
			catch (Exception e) {
				throw new InvalidOperationException(
					$"Stat {typeof(TStat).Name} is not registered", e);
			}
		}
	}
}