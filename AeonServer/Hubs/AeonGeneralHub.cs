using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Aeon.Base;
using System.Diagnostics;
using Polly;

namespace AeonServer;

[Authorize]
public class AeonGeneralHub : AeonHub<AeonGeneralHub.IClient>
{
	private readonly ServerState _state;
	public AeonGeneralHub(ServerState state) => _state = state;


	public async Task<AccountInfo> GetAccountInfo()
	{
		return await Task.FromResult(new AccountInfo { NickName = UserName });
	}
	public override async Task OnConnectedAsync()
{
		_state.Number++;
		_state.Connected(UserID, UserName);
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		_state.Number--;
		_state.Disconnected(UserID);
		await base.OnDisconnectedAsync(exception);
	}

	public interface IClient
	{
		Task StartGame(RoomFullData room);
	}
}


public class AeonHub<T> : Hub<T> where T: class
{
	public string UserID => Context.UserIdentifier!;
	public string UserName => Context.User?.Identity?.Name!;
	public T UserClient => Clients.User(UserID);

	public T PlayerClient(Player player) => Clients.User(player.ID);

	public T RoomClients(Room room) => Clients.Users(room.Players.Select(p => p.ID));
}

public static class AeonHubExtensions
{
	public static T PlayerClient<Thub, T>(this IHubContext<Thub, T> context, Player player)
		where Thub : Hub<T> where T : class => context.Clients.User(player.ID);
	public static T RoomClients<Thub, T>(this IHubContext<Thub, T> context, Room room) 
		where Thub : Hub<T> where T : class => context.Clients.Users(room.Players.Select(p => p.ID));
}


public class TrofUserIdProvider : IUserIdProvider
{
	public virtual string GetUserId(HubConnectionContext connection)
	{
		return connection.User?.Identity?.Name!;
	}
}