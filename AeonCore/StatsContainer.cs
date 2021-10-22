using System;
using System.Collections.Generic;

namespace AeonCore
{

	public class StatsContainer : IReadOnlyStats
	{
		Dictionary<StatType, Stat> _stats = new();

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

		public void Set<TStat>(int value) where TStat : StatType, new()
		{
			try {
				_stats[StatType.Instance<TStat>()] = Stat.Make<TStat>(value);
			} catch (Exception e) {
				throw new InvalidOperationException(
					$"Stat {typeof(TStat).Name} is not registered", e);
			}
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

		internal void AddStat(Stat stat)
		{
			_stats[stat.Behaviour] += stat;
		}
	}
}
