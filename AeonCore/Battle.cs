using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Core
{
	public interface IBattle
	{
		public interface IBattlersProv
		{
			IEnumerable<IBattler> GetBattlers();
		}

		public interface ILogger
		{
			void LogBattlersState(IBattler battler1, IBattler battler2, LogType logType);

			void LogDamage(Damage dmg1to2, Damage dmg2to1);
		}

		public enum LogType
		{
			InitState,
			AfterDamage,
			AfterHealing,
			AfterBattle
		}

		/// <summary>
		/// Запуск драки
		/// </summary>
		/// <returns>Индекс победителя (1, 2…)</returns>
		public int Start();
	}

	public class Battle : IBattle
	{
		IBattler _h1;
		IBattler _h2;
		IBattle.ILogger _logger;

		public Battle(IBattle.IBattlersProv provider, IBattle.ILogger logger = null)
		{
			var list = provider.GetBattlers().ToList();
			_h1 = list[0];
			_h2 = list[1];
			_logger = logger;
		}

		public int Rounds { get; private set; } = 0;

		public int Winner { get; private set; } = 0;

		public int Start()
		{
			_h1.OnBattleStart(_h2);
			_h2.OnBattleStart(_h1);

			const int MAX_ROUNDS = 50;

			_logger?.LogBattlersState(_h1, _h2, IBattle.LogType.InitState);

			while (_h1.IsAlive && _h2.IsAlive && Rounds < MAX_ROUNDS) {
				++Rounds;

				Damage dmg1to2 = _h1.GetDamageTo(_h2);
				Damage dmg2to1 = _h2.GetDamageTo(_h1);

				Damage received1 = _h1.ReceiveDamage(dmg2to1);
				Damage received2 = _h2.ReceiveDamage(dmg1to2);

				_logger?.LogDamage(received2, received1);
				_logger?.LogBattlersState(_h1, _h2, IBattle.LogType.AfterDamage);

				if (!(_h1.IsAlive && _h2.IsAlive)) break;

				_h1.AfterHit(received1);
				_h2.AfterHit(received2);

				_logger?.LogBattlersState(_h1, _h2, IBattle.LogType.AfterHealing);
			}

			_h1.AfterBattle(_h2);
			_h2.AfterBattle(_h1);

			_logger?.LogBattlersState(_h1, _h2, IBattle.LogType.AfterBattle);

			if (Rounds == MAX_ROUNDS) return 0;	
			return Winner = _h1.IsAlive ? 1 : _h2.IsAlive ? 2 : 0;
		}
	}
}
