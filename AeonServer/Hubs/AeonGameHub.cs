using AeonServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AeonServer;

[Authorize]
public class AeonGameHub : Hub<AeonGameHub.IClient>
{
	private readonly ServerState _state;
	private readonly HeroesProvider _heroes;
	public AeonGameHub(ServerState state, HeroesProvider heroes)
	{
		_state = state;
		_heroes = heroes;
	}

	public string UserName => Context.User?.Identity?.Name!;
	public Player Player => _state.Players[UserName];
	public string UserGroup => $"GAME_{Player.Room!.Name}";

	public override async Task OnConnectedAsync()
	{
		if (Player.Room is null) return;
		if (!Player.Room.Status.HasFlag(RoomStatus.InGame)) return;
		await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup);
		await base.OnConnectedAsync();
		Player.Game = _state.Games[Player.Room!.Name];
		if (Player.Room.Players.All(p => p.Game is not null)) {
			Player.Game.Pick();
			await Clients.Group(UserGroup).PickPhaseStarted(await GetAvailiableHeroes());
				//.SendAsync("PickPhaseStarted", await GetAvailiableHeroes());
		}
	}

	public Task<HeroInfo[]> GetAvailiableHeroes() => Task.FromResult(_heroes.GetHeroesInfo());

	public async Task SelectHero(int HeroID)
	{
		if (Player.Game?.Phase == GameState.P.Pick) {
			Player.SelectHero(_heroes.GetHero(HeroID));
			await Clients.Group(UserGroup).HeroSelectedAnyone(new List<HeroSelection>());
				//.SendAsync("HeroSelectedAnyone", new List<HeroSelection>());
		}
	}

	/////
	
	public async Task<HeroModel> GetOwnHero()
	{

	}

	public async Task<OfferModel[]> GetOffers()
	{

	}

	public async Task BuyOffer(int offerId)
	{

	}

	public interface IClient
	{
		Task PickPhaseStarted(HeroInfo[] heroes);
		Task HeroSelectedAnyone(IEnumerable<HeroSelection> selections);
	}
}