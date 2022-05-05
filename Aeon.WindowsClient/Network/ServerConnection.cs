using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace Aeon.WindowsClient;

class ServerConnection
{
	public HubConnection Connection { get; }
	private string? JWToken { get; }


	public ServerConnection(string? token, string url)
	{
		JWToken = token;
		Connection = new HubConnectionBuilder()
				.WithUrl(url, o => o.AccessTokenProvider = () => Task.FromResult(JWToken))
				.Build();
	}

	public async Task Connect() => await Connection.StartAsync();
	public async Task Disconnect() => await Connection.StopAsync();

	async protected Task<T> Request<T>(string method, object obj) => await Connection.InvokeAsync<T>(method, obj);
	async protected Task<T> Request<T>(string method) => await Connection.InvokeAsync<T>(method);
	async protected Task Call(string method) => await Connection.SendAsync(method);
	async protected Task Call(string method, object obj) => await Connection.SendAsync(method, obj);
}
