using Aeon.Core;

namespace AeonServer;

public class Player
{
	public string ID { get; }
	public Room? Room { get; internal set; }
	public GameState? Game { get; internal set; }
	public PlayerData Data { get; internal set; }

	public int? HeroID { get; private set; }
	public string? HeroName { get; private set; }
	public Aeon.Core.Hero? Hero { get; private set; }

	public Models.ShopUpdate? LastShopUpdate { get; internal set; }
	public ShopUpdate GetShopUpdate(ShopUpdate.R response) => LastShopUpdate = new ShopUpdate {
		Hero = Models.Hero.FromAeon(Hero!),
		Offers = Hero!.Shop.Offers.Select((o, id) => o.ToBase(Hero.Stats, id)).ToArray(),
		Response = response,
		CloseIn = Game!.ShopCloseTime
	};

	public Player(string id, string? nickname)
	{
		ID = id;
		Room = null;
		Data = new PlayerData { PlayerName = nickname ?? "[Guest]", IsObserver = false, IsReady = false };
	}

	internal void SelectHero(int id, string name, Aeon.Core.Hero hero)
	{
		HeroID = id;
		HeroName = name;
		Hero = hero;
	}

	internal RoundInfo.Contender Contender => new() { HeroID = HeroName, PlayerName = ID, Points = 0 };

	public override string ToString() => $"{ID} ({HeroName})";
}