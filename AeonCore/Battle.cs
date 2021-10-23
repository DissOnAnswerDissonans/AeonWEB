using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonCore
{
	public class Battle
	{
		public interface IBattlersProv
		{
			IEnumerable<IBattler> GetBattlers();
		}

		IBattler _h1;
		IBattler _h2;

		public Battle(IBattlersProv provider)
		{
			var list = provider.GetBattlers().ToList();
			_h1 = list[0];
			_h2 = list[1];
		}

		public int Start()
		{
			_h1.OnBattleStart(_h2);
			_h2.OnBattleStart(_h1);

			const int MAX_ROUNDS = 50;

			int round = 0;
			
			while(_h1.IsAlive && _h2.IsAlive && round < MAX_ROUNDS) {
				++round;

				Damage dmg1to2 = _h1.GetDamageTo(_h2);
				Damage dmg2to1 = _h2.GetDamageTo(_h1);

				Damage received1 = _h1.ReceiveDamage(dmg2to1);
				Damage received2 = _h2.ReceiveDamage(dmg1to2);

				if (!(_h1.IsAlive && _h2.IsAlive)) break;

				_h1.AfterHit(received1);
				_h2.AfterHit(received2);
			}

			if (round == MAX_ROUNDS) return 0;
			return _h1.IsAlive ? 1 : _h2.IsAlive ? 2 : 0;
		}
	}
}
