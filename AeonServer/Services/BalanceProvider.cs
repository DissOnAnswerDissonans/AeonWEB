using Aeon.Core;
namespace AeonServer.Services;

public interface IBalanceProvider
{
	public BalanceSheet GetBalanceSheet();
}

public class DefaultBalanceProvider : IBalanceProvider
{
	private BalanceSheet _balance;
	public BalanceSheet GetBalanceSheet() => throw new NotImplementedException();

	public DefaultBalanceProvider()
	{
		_balance = new BalanceSheet {

			GlobalBalance = new () {
				["HP" ] = 100,
				["ATT"] = 15,
				["MAG"] = 0,
				["CHA"] = 0,
				["CDM"] = 150,
				["INC"] = 0,
				["BLK"] = 1,
				["ARM"] = 0,
				["REG"] = 1,
			},

			HeroesBalance = new () {
				["Aeon.Heroes:Banker"] = new () {
					["@maxDrop"] = 50,
				},
				["Aeon.Heroes:Beast"] = new() {
					["@dmgBoost"] = .039m,
				},
				["Aeon.Heroes:BloodyElf"] = new() {
					["@moneyBurn"] = 2,
					["@moneyBurnCost"] = 3,
					["@magHitBonus"] = 0.3m,
					["@magHitCost"] = 4,
					["@healingCoeff"] = 0.2m,
					["@healingCost"] = 5,
				},
				["Aeon.Heroes:Cheater"] = new() {
					["@attMultiplier"] = 0.93m,
					["@firstAttX"] = 2.0m,
				},
				["Aeon.Heroes:Fatty"] = new() {
					["@healthMultiplier"] = 1.095m,
					["@regenBonus"] = 2,
				},
				["Aeon.Heroes:Fe11"] = new() {
					["@StartHealthMult"] = 0.5m,
					["@StartAttackMult"] = 2.0m,
					["@InitIncome"] = 2,
					["@BattlesForBonus"] = 10,
				},
				["Aeon.Heroes:Killer"] = new() {
					["@conversionRate"] = 0.15m,
					["@lvlCoeff"] = 75,
					["@attackBonus"] = 10,
				},
				["Aeon.Heroes:Master"] = new() {
					["@vampCoeffStart"] = 0.15m,
					["@vampAdder"] = .006m,
				},
				["Aeon.Heroes:Rogue"] = new() {
					["@rogueHit"] = 0.09m,
					["@enemyHit"] = 0.11m,
					["@battleBonus"] = 0.02m,
				},
				["Aeon.Heroes:Thief"] = new() {
					["@"] = 0,
				},
				["Aeon.Heroes:Tramp"] = new() {
					["@moneyBeg"] = 1.1m,
				},
				["Aeon.Heroes:Trickster"] = new() {
					["@resetSalvage"] = .80m,
				},
				["Aeon.Heroes:Vampire"] = new() {
					["@"] = 0, //UNDONE
				},
				["Aeon.Heroes:Warlock"] = new() {
					["@cost"] = 10,
					["@bonus"] = 17,
				},
				["Aeon.Heroes:Warrior"] = new() {
					["@critDmgBonus"] = 0.5m,
					["@critChaBonus"] = 0.1m,
				},
			},

			StandardOffers = new() {
				Offer<Health>(22, 10),
				Offer<Attack>(3, 7),
				Offer<Magic>(7, 15),
				Offer<CritChance>(5, 15),
				Offer<CritDamage>(50, 50),
				Offer<Income>(2, 13),
				Offer<Block>(2, 4),
				Offer<Armor>(15, 30),
				Offer<Regen>(5, 11),

				Offer<Health>(220, 87, opt: true),
				Offer<Attack>(60, 120, opt: true),
				Offer<Magic>(46, 90, opt: true),
				Offer<CritChance>(40, 104, opt: true),
				Offer<CritDamage>(120, 105, opt: true),
				Offer<Income>(20, 120, opt: true),
				Offer<Block>(80, 130, opt: true),
				Offer<Armor>(66, 120, opt: true),
				Offer<Regen>(62, 115, opt: true),
			}
		};
	}

	private static Aeon.Base.Offer Offer<T>(int amount, int cost, bool opt = false) 
		where T : StatType, new() => new() {
		Cost = cost, IsOpt = opt, StatAmount = new Aeon.Base.Stat {
			RawValue = amount, StatId = StatType.Instance<T>().ID
		}
	};
}
