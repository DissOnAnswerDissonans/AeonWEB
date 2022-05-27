using System.Collections.Generic;
using AeonServer.Models;
using Trof.Connection.Client;

namespace Aeon.WindowsClient;

class Game : ServerConnection
{
	public ClientTx<int> SelectHero { get; } = null!;
	public ClientTx<int> BuyOffer { get; } = null!;
	public ClientTx UseAbility { get; } = null!;
	public ClientTx DoneShopping { get; } = null!;
	public ClientTx LeaveGame { get; } = null!;

	public ClientReq<HeroInfo[]> GetAvailiableHeroes { get; } = null!;

	public ClientRx<List<HeroSelection>> HeroSelectedAnyone { get; } = null!;
	public ClientRx<HeroInfo[]> PickPhaseStarted { get; } = null!;
	public ClientRx<ShopUpdate> ShopUpdated { get; } = null!;
	public ClientRx<RoundInfo> NewRoundStarted { get; } = null!;
	public ClientRx<BattleTurn> NewBattleTurn { get; } = null!;
	public ClientRx<RoundScoreSummary> NewRoundSummary { get; } = null!;
	public ClientRx<FinalResult> GameOver { get; } = null!;
}