using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Collections.Generic;
using AeonServer.Models;

namespace Aeon.WindowsClient;

class Game : ServerConnection
{
	public Game(string? token, string url) : base(token, $"{url}/aeon/game") 
	{
		HeroSelectedAnyone = Observable.Create<List<HeroSelection>>
			(obs => Connection.On<List<HeroSelection>>("HeroSelectedAnyone", s => obs.OnNext(s)));
		PickPhaseStarted = Observable.Create<HeroInfo[]>
			(obs => Connection.On<HeroInfo[]>("PickPhaseStarted", s => obs.OnNext(s)));
		ShopUpdated = Observable.Create<ShopUpdate>
			(obs => Connection.On<ShopUpdate>("ShopUpdated", s => obs.OnNext(s)));
		NewRoundStarted = Observable.Create<RoundInfo>
			(obs => Connection.On<RoundInfo>("NewRound", s => obs.OnNext(s)));

		ShopUpdated.Subscribe(s => LastShopUpdate = s);
		NewRoundStarted.Subscribe(s => LastRoundInfo = s);
	}

	public IObservable<List<HeroSelection>> HeroSelectedAnyone { get; }
	public IObservable<HeroInfo[]> PickPhaseStarted { get; }
	public IObservable<ShopUpdate> ShopUpdated { get; }
	public IObservable<RoundInfo> NewRoundStarted { get; }

	public async Task<HeroInfo[]> GetAvailiableHeroes() => await Request<HeroInfo[]>("GetAvailiableHeroes");
	public async Task SelectHero(int heroID) => await Call("SelectHero", heroID);

	public ShopUpdate? LastShopUpdate { get; private set; }
	public RoundInfo? LastRoundInfo { get; private set; }
}