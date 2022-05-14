using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Collections.Generic;
using AeonServer.Models;
using Aeon.Core;

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
		NewBattleTurn = Observable.Create<BattleTurn>
			(obs => Connection.On<BattleTurn>("NewBattleTurn", s => obs.OnNext(s)));
		NewRoundSummary = Observable.Create<RoundScoreSummary>
			(obs => Connection.On<RoundScoreSummary>("NewRoundSummary", s => obs.OnNext(s)));

		ShopUpdated.Subscribe(s => LastShopUpdate = s);
		NewRoundStarted.Subscribe(s => LastRoundInfo = s);
		NewRoundSummary.Subscribe(s => LastSummary = s);
	}

	public IObservable<List<HeroSelection>> HeroSelectedAnyone { get; }
	public IObservable<HeroInfo[]> PickPhaseStarted { get; }
	public IObservable<ShopUpdate> ShopUpdated { get; }
	public IObservable<RoundInfo> NewRoundStarted { get; }
	public IObservable<BattleTurn> NewBattleTurn { get; }
	public IObservable<RoundScoreSummary> NewRoundSummary { get; }

	public async Task<HeroInfo[]> GetAvailiableHeroes() => await Request<HeroInfo[]>("GetAvailiableHeroes");
	public async Task SelectHero(int heroID) => await Call("SelectHero", heroID);
	public async Task BuyOffer(object id) => await Call("BuyOffer", id);
	public async Task DoneShopping() => await Call("DoneShopping");

	public ShopUpdate? LastShopUpdate { get; private set; }
	public RoundInfo? LastRoundInfo { get; private set; }
	public RoundScoreSummary? LastSummary { get; private set; }
}