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
			Player.SelectHero(HeroID, _heroes.HeroesList[HeroID], _heroes.GetHero(HeroID));
			await PicksUpdate(Player.Game);
			if (Player.Game.Players.All(p => p.Hero is not null))
				_ = Player.Game.GameStart();
		}
	}

	public async Task PicksUpdate(GameState game) => await Clients.Group(game.SRGroup).HeroSelectedAnyone(
		game.Players.Select(p => new HeroSelection {
			Nickname = p.Data.PlayerName,
			Hero = _heroes.GetHeroInfo(p.HeroID)
		}));


	public async Task BuyOffer(int offerId)
	{
		var result = Player.Hero?.TryBuyOffer(Player.Hero.Shop.Offers[offerId]);
		await Clients.Caller.ShopUpdated(Player.GetShopUpdate(result switch {
			true  => ShopUpdate.R.OK,
			false => ShopUpdate.R.NotEnough,
			null  => ShopUpdate.R.OtherError
		}));
	}

	public async Task DoneShopping()
	{
		await Clients.Caller.ShopUpdated(Player.GetShopUpdate(ShopUpdate.R.Closed));
		if (Player.Game!.Players.Select(p => p.LastShopUpdate).All(u => u?.Response == ShopUpdate.R.Closed))
			Player.Game!.ShopCTS!.Cancel();
	}

	//////


		
	public interface IClient
	{
		Task PickPhaseStarted(HeroInfo[] heroes);
		Task HeroSelectedAnyone(IEnumerable<HeroSelection> selections);
		Task ShopUpdated(ShopUpdate shop);
		Task NewRound(RoundInfo round);
		Task NewBattleTurn(BattleTurn turn);
		Task NewRoundSummary(RoundScoreSummary summary);
	}
}