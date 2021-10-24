using System;
using System.Linq;
using System.Collections.Generic;

namespace Aeon.Core
{

	public class StatsContainer : IReadOnlyStats
	{
		Dictionary<StatType, Stat> _stats = new();
		Dictionary<StatTypeDynamic, DynStat> _dynStats = new();

		public Stat this[StatType type] => _stats[type];

		public bool Register<TStat>(int value) where TStat : StatType, new()
		{
			Stat stat = Stat.Make<TStat>(value);
			try {
				_stats.Add(stat.Behaviour, stat);
			} catch (Exception e) {
#if DEBUG
				throw new InvalidOperationException(
					$"Stat {stat.Behaviour} is already registered", e);
#endif
				return false;
			}
			return true;
		}

		public bool RegisterDyn<TStat>(int value) where TStat : StatTypeDynamic, new()
		{
			if (Register<TStat>(value)) {
				DynStat dyn = DynStat.Make<TStat>(0);
				_dynStats.Add(dyn.Behaviour, dyn);
				return true;
			}
			return false;
		}

		public void Set<TStat>(int value) where TStat : StatType, new()
		{
			if (!_stats.ContainsKey(StatType.Instance<TStat>())) {
#if DEBUG
				throw new InvalidOperationException(
					$"Stat {typeof(TStat).Name} is not registered");
#endif
			}
			_stats[StatType.Instance<TStat>()] = Stat.Make<TStat>(value);
		}

		public Stat Get<TStat>() where TStat : StatType, new()
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



		internal void AddStat(Stat stat)
		{
			try {
				_stats[stat.Behaviour] += stat;
			} catch (KeyNotFoundException) {
				throw new InvalidOperationException(
					$"Stat {stat.StatType.GetType()} is not registered");
			}
		}

		internal DynStat Modify<TStat>(int delta) where TStat : StatTypeDynamic, new()
		{
			DynStat stat = GetDyn<TStat>();
			stat.SetValue(stat.Value + delta, this);
			return _dynStats[StatType.Instance<TStat>()] = stat;
		}
	}
}
