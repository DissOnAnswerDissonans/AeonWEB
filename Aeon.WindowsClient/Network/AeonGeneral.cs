using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Aeon.WindowsClient;

class AeonGeneral : ServerConnection
{
	public AeonGeneral(string? token, string url) : base(token, $"{url}/aeon") 
	{
		OnGameStart = Observable.Create<RoomFullData>
			(obs => Connection.On<RoomFullData>("StartGame", s => obs.OnNext(s)));
	}

	public async Task<AccountInfo> GetAccountInfo() => await Request<AccountInfo>("GetAccountInfo");

	public IObservable<RoomFullData> OnGameStart { get; }
}
