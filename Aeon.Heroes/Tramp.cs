using Aeon.Core;

namespace Aeon.Heroes
{
	/// <summary>
	/// Герой ранней и средней стадий игры. Перед каждым ударом
	/// увеличивает количество своей игровой валюты на 1.1.
	/// </summary>
	public class Tramp : Hero
	{
		private static decimal moneyBeg = 1.1m;
		private decimal _coc = 0;
		private int _grabbed;

		public override Damage ReceiveDamage(Damage damage)
		{
			_coc += moneyBeg;
			int beg = (int) _coc;
			Wage(beg);
			_coc -= beg;
			_grabbed += beg;
			return base.ReceiveDamage(damage);
		}

		public override string AbilityText => $"Намошнил ${_grabbed}";
	}
}