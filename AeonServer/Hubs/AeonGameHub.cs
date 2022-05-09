using Aeon.Core;
using AeonServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AeonServer;

[Authorize]
public class AeonGameHub : AeonHub<AeonGameHub.IClient>
{
	private readonly GameProvider _games;
	private readonly HeroesProvider _heroes;
	public AeonGameHub(GameProvider games, HeroesProvider heroes)
	{
		_games = games;
		_heroes = heroes;
	}

	public Player Player => _games.GetPlayer(UserID);
	public string UserGroup => $"GAME_{_games.FromUser(UserID).Name}";

	public override async Task OnConnectedAsync()
	{
		if (Player.Room is null) return;
		if (!Player.Room.Status.HasFlag(RoomStatus.InGame)) return;
		Player.Game = _games.FromRoom(Player.Room.Name);
		await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup);
		await base.OnConnectedAsync();

		if (Player.Room.Players.All(p => p.Game is not null)) {
			Player.Game.Pick();
			await Clients.Group(Player.Game.SRGroup).PickPhaseStarted(await GetAvailiableHeroes());
			await PicksUpdate(Player.Game);
		}
	}

	public Task<HeroInfo[]> GetAvailiableHeroes() => Task.FromResult(_heroes.GetHeroesInfo());

	public async Task SelectHero(int HeroID)
	{
		if (Player.Game?.Phase == GameState.P.Pick && Player.Hero is null) {
			Player.SelectHero(HeroID, _heroes.GetHero(HeroID));
			await PicksUpdate(Player.Game);
			if (Player.Game.Players.All(p => p.Hero is not null))
				_ = GameStart(Player.Game);
		}
	}

	public async Task PicksUpdate(GameState game) => await Clients.Group(game.SRGroup).HeroSelectedAnyone(
		game.Players.Select(p => new HeroSelection {
			Nickname = p.Data.PlayerName,
			Hero = _heroes.GetHeroInfo(p.HeroID)
		}));

	/////

	//public async Task<HeroModel> GetOwnHero()
	//{

	//}

	//public async Task<OfferModel[]> GetOffers()
	//{

	//}

	public async Task BuyOffer(int offerId)
	{
		var result = Player.Hero?.TryBuyOffer(Player.Hero.Shop.Offers[offerId]);
		await Clients.Caller.ShopUpdated(Player.GetShopUpdate(result switch {
			true  => ShopUpdate.R.OK,
			false => ShopUpdate.R.NotEnough,
			null  => ShopUpdate.R.OtherError
		}));
	}

	//////

	private async Task NewRoundStart(GameState game)
	{
		game.NewRound();
		await Clients.Group($"GAME_{game.Name}").NewRound(game.GetRound());
		await StartShopping(game);
	}

	private async Task StartShopping(GameState game)
	{
		//List<Task> t = new();
		foreach (var player in game.Players) {
			var upd = new ShopUpdate {
				Hero = Models.Hero.FromAeon(player.Hero!),
				Offers = player.Hero!.Shop.Offers.Select(o => o.ToBase(player.Hero!.Stats)).ToArray(),
				Response = ShopUpdate.R.Opened
			};

			var cyka = System.Text.Json.JsonSerializer.Serialize(upd);
			var cyka2 = System.Text.Json.JsonSerializer.Deserialize<ShopUpdate>(cyka);

			await Clients.User(player.ID).ShopUpdated(upd);
		}
		//await Task.WhenAll(t.ToArray());
	}
		
	public interface IClient
	{
		Task PickPhaseStarted(HeroInfo[] heroes);
		Task HeroSelectedAnyone(IEnumerable<HeroSelection> selections);
		Task ShopUpdated(ShopUpdate shop);
		Task NewRound(RoundInfo round);
	}

	private async Task GameStart(GameState game)
	{
		await Task.Delay(3_000);
		while (true) {
			var s = NewRoundStart(game);
			await Task.WhenAll(s, Task.Delay(30_000));
		}
	}
}