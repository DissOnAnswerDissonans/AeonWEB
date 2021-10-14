using System;
using System.Collections.Generic;

namespace AeonCore
{
	public class StatsContainer
	{
		static Dictionary<string, Type> _strNames = new();

		Dictionary<Type, Stat> _stats = new();

		public Stat this[string s] => _stats[_strNames[s]];

		public bool Register<TStat>(int value) where TStat : Stat, new()
		{
			Stat stat = new TStat {
				Value = value
			};
			try {
				_stats.Add(typeof(TStat), stat);
			} catch (Exception e) {
#if DEBUG
				throw new InvalidOperationException(
					$"Stat {stat.StatName} is already registered", e);
#endif
				return false;
			}
			if (!_strNames.ContainsKey(stat.StatName))
				_strNames[stat.StatName] = typeof(TStat);
			return true;
		}

		public void Set<TStat>(int value) where TStat : Stat
		{
			try {
				_stats[typeof(TStat)].Value = value;
			} catch (Exception e) {
				throw new InvalidOperationException(
					$"Stat {typeof(TStat).Name} is not registered", e);
			}
		}

		public TStat Get<TStat>() where TStat : Stat
		{
			try {
				return (TStat) _stats[typeof(TStat)];
			}
			catch (Exception e) {
				throw new InvalidOperationException(
					$"Stat {typeof(TStat).Name} is not registered", e);
			}
		}

		public int Value<TStat>() where TStat : Stat => Get<TStat>().Value;
		public double Converted<TStat>() where TStat : Stat => Get<TStat>().Converted;
	}
}
