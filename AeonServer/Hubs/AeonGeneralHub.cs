using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Aeon.Base;

namespace AeonServer;

[Authorize]
public class AeonGeneralHub : Hub
{
	private readonly ServerState _state;
	public AeonGeneralHub(ServerState state) => _state = state;
	public string UserName => Context.User?.Identity?.Name!;

	public async Task<AccountInfo> GetAccountInfo()
	{
		return await Task.FromResult(new AccountInfo { NickName = UserName });
	}
	public override async Task OnConnectedAsync()
{
		_state.Number++;
		_state.Connected(UserName);
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		_state.Number--;
		_state.Disconnected(UserName);
		await base.OnDisconnectedAsync(exception);
	}
}

