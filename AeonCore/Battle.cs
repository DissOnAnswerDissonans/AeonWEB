using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonCore
{
	public class Battle
	{
		Player _p1;
		Player _p2;

		Hero _h1;
		Hero _h2;

		public Battle(Game game)
		{
			_p1 = game.Player1;
			_p2 = game.Player2;
			_h1 = _p1.Hero;
			_h2 = _p2.Hero;
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
