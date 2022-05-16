using Aeon.Core;
using static Aeon.Core.Hero;
namespace AeonServer.Services;

public interface IBalanceProvider
{
	internal BalanceSheet GetBalanceSheet();

	public BalanceValue ValueForHero(Aeon.Core.Hero hero, string key);
	public Func<Shop> ShopFactory { get; }
}

public class DefaultBalanceProvider : IBalanceProvider
{
	private BalanceSheet _balance;
	BalanceSheet IBalanceProvider.GetBalanceSheet() => _balance;

	public Func<Shop> ShopFactory => () => new BalancedShop(_balance);

	public DefaultBalanceProvider()
	{
		_balance = new BalanceSheet {

			GlobalBalance = new() {
				[Health] = 100,
				[Attack] = 15,
				[Magic] = 0,
				[CritChance] = 0,
				[CritDamage] = 150,
				[Income] = 0,
				[Block] = 1,
				[Armor] = 0,
				[Regen] = 1,
			},

			HeroesBalance = new() {
				["Aeon.Heroes:Banker"] = new() {
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
					["@startHealthMult"] = 0.5m,
					["@startAttackMult"] = 2.0m,
					["@initIncome"] = 2,
					["@battlesForBonus"] = 10,
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
				Offer(Health, 22, 10),
				Offer(Attack, 3, 7),
				Offer(Magic, 7, 15),
				Offer(CritChance, 5, 15),
				Offer(CritDamage, 50, 50),
				Offer(Income, 2, 13),
				Offer(Block, 2, 4),
				Offer(Armor, 15, 30),
				Offer(Regen, 5, 11),

				Offer(Health, 220, 87, opt: true),
				Offer(Attack, 60, 120, opt: true),
				Offer(Magic, 46, 90, opt: true),
				Offer(CritChance, 40, 104, opt: true),
				Offer(CritDamage, 120, 105, opt: true),
				Offer(Income, 20, 120, opt: true),
				Offer(Block, 80, 130, opt: true),
				Offer(Armor, 66, 120, opt: true),
				Offer(Regen, 62, 115, opt: true),
			}
		};
	}

	public BalanceValue ValueForHero(HeroInfo hero, string key) =>
		_balance.HeroesBalance[hero.NameID][key];

	public BalanceValue ValueForHero(Aeon.Core.Hero hero, string key) =>
		_balance.HeroesBalance[hero.ID][key];

	private static OfferData Offer(string id, int amount, int cost, bool opt = false) => new() {
		Cost = cost, IsOpt = opt, StatAmount = new StatData {
			RawValue = amount, StatId = id
		}
	};
}

public sealed class BalancedShop : Shop
{
	public BalancedShop(BalanceSheet balance) : base() => balance.StandardOffers
		.ForEach(o => AddOffer(o.StatAmount.StatId, o.StatAmount.RawValue, o.Cost, o.IsOpt));
}