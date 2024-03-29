﻿namespace Aeon.Core;

public interface IBattler
{
	string ID { get; }
	/// <summary>
	/// Характеристики гладиатора
	/// </summary>
	IStatContext StatsRO { get; }

	/// <summary>
	/// Жив ли этот гладиаторъ
	/// </summary>
	bool IsAlive { get; }

	/// <summary>
	/// Вызвать до начала боя
	/// </summary>
	/// <param name="enemy">Противник</param>
	void OnBattleStart(IBattler enemy);

	/// <summary>
	/// Вызвать для расчета наносимого урона
	/// </summary>
	/// <param name="enemy">Кто получит урон</param>
	/// <returns>Урон до сопротивлений</returns>
	Damage GetDamageTo(IBattler enemy);

	/// <summary>
	/// Вызвать для получения повреждений
	/// </summary>
	/// <param name="damage">Урон до сопротивлений</param>
	/// <returns>Общий полученный урон</returns>
	Damage ReceiveDamage(Damage damage);

	/// <summary>
	/// Вызвать после получения урона
	/// </summary>
	/// <param name="enemyHit">Полученный урон</param>
	/// <param name="ourHit">Нанесенный урон</param>
	void AfterHit(Damage enemyHit, Damage ourHit);

	/// <summary>
	/// Вызвать после боя
	/// </summary>
	/// <param name="enemy">Противник</param>
	void AfterBattle(IBattler enemy, bool isWin);
}
