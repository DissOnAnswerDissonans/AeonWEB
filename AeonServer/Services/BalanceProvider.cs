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
				[":Banker"] = new() {
					["@maxDrop"] = 50,
				},
				[":Beast"] = new() {
					["@dmgBoost"] = .039m,
				},
				[":BloodyElf"] = new() {
					["@moneyBurn"] = 2,
					["@moneyBurnCost"] = 3,
					["@magHitBonus"] = 0.3m,
					["@magHitCost"] = 4,
					["@healingCoeff"] = 0.2m,
					["@healingCost"] = 5,
				},
				[":Cheater"] = new() {
					["@attMultiplier"] = 0.93m,
					["@firstAttX"] = 2.0m,
				},
				[":Fatty"] = new() {
					["@healthMultiplier"] = 1.095m,
					["@regenBonus"] = 2,
				},
				[":Fe11"] = new() {
					["@startHealthMult"] = 0.5m,
					["@startAttackMult"] = 2.0m,
					["@initIncome"] = 2,
					["@battlesForBonus"] = 10,
				},
				[":Killer"] = new() {
					["@conversionRate"] = 0.15m,
					["@lvlCoeff"] = 75,
					["@attackBonus"] = 10,
				},
				[":Master"] = new() {
					["@vampCoeffStart"] = 0.15m,
					["@vampAdder"] = .006m,
				},
				[":Rogue"] = new() {
					["@rogueHit"] = 0.09m,
					["@enemyHit"] = 0.11m,
					["@battleBonus"] = 0.02m,
				},
				[":Thief"] = new() {
					["@"] = 0,
				},
				[":Tramp"] = new() {
					["@moneyBeg"] = 1.1m,
				},
				[":Trickster"] = new() {
					["@resetSalvage"] = .80m,
				},
				[":Vampire"] = new() {
					["@"] = 0, //UNDONE
				},
				[":Warlock"] = new() {
					["@cost"] = 10,
					["@bonus"] = 17,
				},
				[":Warrior"] = new() {
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