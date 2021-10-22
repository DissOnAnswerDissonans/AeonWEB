using System;
using System.Linq;
using System.Collections.Generic;

namespace AeonCore
{

	public class StatsContainer : IReadOnlyStats
	{
		Dictionary<StatType, Stat> _stats = new();
		Dictionary<StatTypeDynamic, DynStat> _dynStats = new();


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
			_stats[stat.Behaviour] += stat;
		}

		internal DynStat Modify<TStat>(int delta) where TStat : StatTypeDynamic, new()
		{
			DynStat stat = GetDyn<TStat>();
			stat.Value += delta;
			return _dynStats[StatType.Instance<TStat>()] = stat;
		}

		internal void OnBattleStart(Hero hero, Hero enemy)
		{
			foreach (StatType tStat in _stats.Keys) {
				Stat k = _stats[tStat];
				if (tStat.OnBattleStart(ref k, hero, enemy))
					_stats[tStat] = k;
			}

			foreach (StatTypeDynamic tStat in _dynStats.Keys) {
				DynStat k = _dynStats[tStat];
				if (tStat.OnBattleStart(ref k, hero, enemy))
					_dynStats[tStat] = k;
			}
		}

		internal void AfterHit(Hero hero, Damage enemyHit)
		{
			foreach (StatType tStat in _stats.Keys) {
				Stat k = _stats[tStat];
				if (tStat.AfterHit(ref k, hero, enemyHit))
					_stats[tStat] = k;
			}

			foreach (StatTypeDynamic tStat in _dynStats.Keys) {
				DynStat k = _dynStats[tStat];
				if (tStat.AfterHit(ref k, hero, enemyHit))
					_dynStats[tStat] = k;
			}
		}


	}
}
