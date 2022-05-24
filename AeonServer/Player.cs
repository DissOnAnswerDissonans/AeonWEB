using Aeon.Core;

namespace AeonServer;


abstract public class Player
{
	public string ID { get; protected set; } = null!;

	public Room? Room { get; internal set; }
	public GameState? Game { get; internal set; }

	public int? HeroID { get; protected set; }
	public string? HeroName { get; protected set; }
	public Aeon.Core.Hero? Hero { get; protected set; }

	internal RoundInfo.Contender Contender => new() { HeroID = HeroName, PlayerName = ID, Points = 0 };
	public override string ToString() => $"{ID} ({HeroName})";
}

public class PlayerClient : Player
{
	public ClientData Data { get; internal set; }

	public ShopUpdate? LastShopUpdate { get; internal set; }
	public ShopUpdate GetShopUpdate(ShopUpdate.R response) => LastShopUpdate = new ShopUpdate {
		Hero = Converters.FromAeon(Hero!),
		Offers = Hero!.Shop.Offers.Select((o, id) => o.ToBase(Hero.Stats, id)).ToArray(),
		Response = response,
		CloseIn = Game!.ShopCloseTime
	};

	public PlayerClient(string id, string? nickname)
	{
		ID = id;
		Room = null;
		Data = new ClientData { PlayerName = nickname ?? "[Guest]", IsObserver = false, IsReady = false };
	}

	internal void SelectHero(int id, string name, Aeon.Core.Hero hero)
	{
		HeroID = id;
		HeroName = name;
		Hero = hero;
	}

	internal void Reset()
	{
		Game = null;
		Room = null;
		Hero = null;
		HeroName = null;
		HeroID = null;
		Data = new ClientData { PlayerName = Data.PlayerName, IsObserver = false, IsReady = false };
	}
}

public class PlayerBot : Player
{
	public PlayerBot(string name) => ID = $"<BOT>{name}";
}