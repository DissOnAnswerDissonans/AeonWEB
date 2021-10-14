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
	}
}
