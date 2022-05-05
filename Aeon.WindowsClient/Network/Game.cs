using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Collections.Generic;
using AeonServer;

namespace Aeon.WindowsClient;

class Game : ServerConnection
{
	public Game(string? token, string url) : base(token, $"{url}/aeon/game") 
	{
		HeroSelectedAnyone = Observable.Create<List<HeroSelection>>
			(obs => Connection.On<List<HeroSelection>>("HeroSelectedAnyone", s => obs.OnNext(s)));
		PickPhaseStarted = Observable.Create<HeroInfo[]>
			(obs => Connection.On<HeroInfo[]>("PickPhaseStarted", s => obs.OnNext(s)));
	}

	public IObservable<List<HeroSelection>> HeroSelectedAnyone { get; }
	public IObservable<HeroInfo[]> PickPhaseStarted { get; }

	public async Task<HeroInfo[]> GetAvailiableHeroes() => await Request<HeroInfo[]>("GetAvailiableHeroes");
	public async Task SelectHero(int heroID) => await Call("SelectHero", heroID);
}