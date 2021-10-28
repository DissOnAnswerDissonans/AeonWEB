using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Герой ранней и средней стадий игры. Перед каждым ударом
	/// увеличивает количество своей игровой валюты на 1.1.
	/// </summary>
	public class Tramp : Hero
	{
		const decimal MONEY_B = 1.1m;
		decimal _coc = 0;
		int _grabbed = 0;

		public override Damage ReceiveDamage(Damage damage)
		{
			_coc += MONEY_B;
			int beg = (int) _coc;
			Wage(beg);
			_coc -= beg;
			_grabbed += beg;
			return base.ReceiveDamage(damage);
		}

		public override string AbilityText => $"Намошнил ${_grabbed}";
	}
}
