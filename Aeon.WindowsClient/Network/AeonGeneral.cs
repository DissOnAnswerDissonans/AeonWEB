using Trof.Connection.Client;

namespace Aeon.WindowsClient;

class AeonGeneral : ServerConnection
{
	public ClientReq<AccountInfo> GetAccountInfo { get; } = null!;

	public ClientRx<RoomFullData> StartGame { get; } = null!;
}
