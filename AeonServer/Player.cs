using Aeon.Core;

namespace AeonServer;

public class Player
{
	public string ID { get; }
	public Room? Room { get; internal set; }
	public GameState? Game { get; internal set; }
	public PlayerData Data { get; internal set; }

	public int? HeroID { get; private set; }
	public Aeon.Core.Hero? Hero { get; private set; }

	public Models.ShopUpdate? LastShopUpdate { get; internal set; }
	public ShopUpdate GetShopUpdate(ShopUpdate.R response) => new ShopUpdate {
		Hero = Models.Hero.FromAeon(Hero!),
		Offers = Hero!.Shop.Offers.Select(o => o.ToBase(Hero.Stats)).ToArray(),
		Response = response
	};

	public Player(string id, string? nickname)
	{
		ID = id;
		Room = null;
		Data = new PlayerData { PlayerName = nickname ?? "[Guest]", IsObserver = false, IsReady = false };
	}

	internal void SelectHero(int id, Aeon.Core.Hero hero)
	{
		HeroID = id;
		Hero = hero;
	}
}