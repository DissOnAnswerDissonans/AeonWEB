using System;
using System.Collections.Generic;

namespace AeonCore
{

	public class StatsContainer : IReadOnlyStats
	{
		static Dictionary<string, Type> _strNames = new();

		Dictionary<Type, StatBehaviour> _stats = new();

		public StatBehaviour this[string s] => _stats[_strNames[s]];

		public bool Register<TStat>(int value) where TStat : StatBehaviour, new()
		{
			StatBehaviour stat = new TStat {
				Value = value
			};
			try {
				_stats.Add(typeof(TStat), stat);
			} catch (Exception e) {
#if DEBUG
				throw new InvalidOperationException(
					$"Stat {stat.StatID} is already registered", e);
#endif
				return false;
			}
			if (!_strNames.ContainsKey(stat.StatID.Name)) // HACK: эту хню со строками надо потом порешать
				_strNames[stat.StatID.Name] = typeof(TStat);
			return true;
		}

		public void Set<TStat>(int value) where TStat : StatBehaviour
		{
			try {
				_stats[typeof(TStat)].Value = value;
			} catch (Exception e) {
				throw new InvalidOperationException(
					$"Stat {typeof(TStat).Name} is not registered", e);
			}
		}

		public TStat Get<TStat>() where TStat : StatBehaviour
		{
			try {
				return (TStat) _stats[typeof(TStat)];
			}
			catch (Exception e) {
				throw new InvalidOperationException(
					$"Stat {typeof(TStat).Name} is not registered", e);
			}
		}

		internal void AddStat(StatBehaviour stat)
		{
			_stats[stat.StatID].Add(stat);
		}
	}
}
