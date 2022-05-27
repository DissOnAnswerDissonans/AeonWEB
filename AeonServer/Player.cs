using Aeon.Core;
using static Aeon.Core.Hero;

namespace AeonServer;


abstract public class Player
{
	public string ID { get; protected set; } = null!;

	public Room? Room { get; internal set; }
	public GameState? Game { get; internal set; }

	public ClientData Data { get; internal set; }

	public string? HeroName { get; protected set; }
	public Aeon.Core.Hero? Hero { get; protected set; }

	internal RoundInfo.Contender Contender => new() { HeroID = HeroName, PlayerName = ID, Points = 0 };
	public override string ToString() => $"{ID} ({HeroName})";

	internal abstract void Reset();
	internal abstract void OnGameStart(GameState s);

	internal void SelectHero(Aeon.Core.Hero hero)
	{
		Hero = hero;
		HeroName = hero.ID;
	}
}

public class PlayerClient : Player
{
	public ShopUpdate? LastShopUpdate { get; internal set; }
	public ShopUpdate GetShopUpdate(ShopUpdate.R response) => LastShopUpdate = new ShopUpdate {
		Hero = Converters.FromAeon(Hero!),
		Offers = Hero!.Shop.Offers.Select((o, id) => o.ToBase(Hero.Stats, id)).ToArray(),
		Response = response,
		CloseIn = Game!.ShopCloseTime,
		AbilityText = Hero.AbilityText
	};

	public PlayerClient(string id, string? nickname)
	{
		ID = id;
		Room = null;
		Data = new ClientData { PlayerName = nickname ?? "[Guest]", IsObserver = false, IsReady = false };
	}

	internal override void Reset()
	{
		Game = null;
		Room = null;
		Hero = null;
		HeroName = null;
		Data = new ClientData { PlayerName = Data.PlayerName, IsObserver = false, IsReady = false };
	}

	internal override void OnGameStart(GameState s) => Data.IsReady = false;
}

public class PlayerBot : Player
{
	public PlayerBot(string name, Room room)
	{
		ID = $"<BOT>{name}";
		Data = new ClientData { PlayerName = ID, IsObserver = false, IsReady = true };
		Room = room;
	}

	internal override void Reset()
	{
		Game = null;
		Room = null;
		Hero = null;
		HeroName = null;
		Data = new ClientData { PlayerName = ID, IsObserver = false, IsReady = true };
	}

	protected int BuyStat(string id, int maxSpend)
	{
		var offers = Hero!.Shop.Offers.Where(o => o.StatID == id && o.Cost <= maxSpend);
		if (offers != null && offers.Any()) {
			Offer offer = offers.MaxBy(x => x.Cost)!;
			Hero!.TryBuyOffer(offer);
			return offer.Cost;
		}
		return 0;
	}

	protected int BuyStatOn(string stat) => BuyStatOn(stat, Hero!.Money);
	protected int BuyStatOn(string stat, int cost)
	{
		cost = Math.Min(cost, Hero!.Money);
		int r, add = cost;
		do {
			r = BuyStat(stat, cost);
			cost -= r;
		} while (r != 0);
		return add - cost;
	}

	public virtual void AutoBuy()
	{
		// 404 или 604 каждый ход
		BuyStatOn(Magic, 60);
		BuyStatOn(Health);
	}

	internal void AutoSelectHero(HeroInfo[] heroes, Services.HeroesProvider _heroes) => 
		SelectHero(_heroes.GetHero(heroes[Random.Shared.Next(heroes.Length)].ID));

	internal override void OnGameStart(GameState s) => Game = s;
}