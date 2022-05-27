using Aeon.Core;
using AeonServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AeonServer;

[Authorize]
public class AeonGameHub : AeonHub<AeonGameHub.IClient>
{
	private readonly ServerState _state;
	private readonly GameProvider _games;
	private readonly HeroesProvider _heroes;
	public AeonGameHub(ServerState state, GameProvider games, HeroesProvider heroes)
	{
		_state = state;
		_games = games;
		_heroes = heroes;
	}

	public PlayerClient Player => _games.GetPlayer(UserID);
	public string UserGroup => $"GAME_{_games.FromUser(UserID).Name}";

	public override async Task OnConnectedAsync()
	{
		if (Player.Room is null) return;
		if (!Player.Room.Status.HasFlag(RoomStatus.InGame)) return;
		Player.Game = _games.FromRoom(Player.Room.Name);
		await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup);
		await base.OnConnectedAsync();

		if (Player.Room.Players.All(p => p.Game is not null)) {
			HeroInfo[] heroes = await GetAvailiableHeroes();
			Player.Game.Pick(heroes, _heroes);
			await Clients.Group(Player.Game.SRGroup).PickPhaseStarted(heroes);
			await PicksUpdate(Player.Game);
		}
	}

	public async Task LeaveGame()
	{
		GameState? game = Player.Game;
		if (game is null) return;
		game.PlayerLeft(Player);
		_state.LeaveRoom(Player.ID);
		Player.Reset();
	}

	public Task<HeroInfo[]> GetAvailiableHeroes() => Task.FromResult(_heroes.GetHeroesInfo());

	public async Task SelectHero(int HeroID)
	{
		if (Player.Game?.Phase == GameState.P.Pick && Player.Hero is null) {
			Player.SelectHero(_heroes.GetHero(HeroID));
			await PicksUpdate(Player.Game);
			if (Player.Game.Players.All(p => p.Hero is not null))
				_ = Player.Game.GameStart();
		}
	}

	public async Task PicksUpdate(GameState game) => await Clients.Group(game.SRGroup).HeroSelectedAnyone(
		game.Clients.Select(p => new HeroSelection {
			Nickname = p.Data.PlayerName,
			Hero = _heroes.GetHeroInfo(p.HeroName)
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
		if (Player.Game!.Clients.Select(p => p.LastShopUpdate).All(u => u?.Response == ShopUpdate.R.Closed))
			Player.Game!.ShopCTS!.Cancel();
	}

	public async Task UseAbility()
	{
		bool? result = Player.Hero?.UseAbility();
		await Clients.Caller.ShopUpdated(Player.GetShopUpdate(result switch {
			true => ShopUpdate.R.AbilityOK,
			false => ShopUpdate.R.AbilityError,
			null => ShopUpdate.R.OtherError
		}));
	}

	//////


		
	public interface IClient
	{
		Task PickPhaseStarted(HeroInfo[] heroes);
		Task HeroSelectedAnyone(IEnumerable<HeroSelection> selections);
		Task ShopUpdated(ShopUpdate shop);
		Task NewRoundStarted(RoundInfo round);
		Task NewBattleTurn(BattleTurn turn);
		Task NewRoundSummary(RoundScoreSummary summary);
		Task GameOver(FinalResult result);
	}
}