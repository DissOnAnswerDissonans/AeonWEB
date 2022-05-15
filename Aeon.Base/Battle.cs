using System;
using System.Collections.Generic;
using System.Text;

namespace Aeon.Base
{
	public class BattleTurn
	{
		public int TurnNumber { get; set; }
		public T TurnType { get; set; }
		public BattleHero Hero { get; set; }
		public EnemyHero Enemy { get; set; }
		public int NextTurnAfterMS { get; set; }

		public enum T { Init, Attack, Heal, End }
	}

	public class BattleHero
	{
		public string HeroId { get; set; }
		public int Health { get; set; }
		public int MaxHealth { get; set; }
		public int ExpectedDamage { get; set; }
		public int ExpectedCrit { get; set; }
		public float BoostBonus { get; set; }
	}

	public class EnemyHero
	{
		public string HeroId { get; set; }
		public int Health { get; set; }
		public int MaxHealth { get; set; }
	}
}
