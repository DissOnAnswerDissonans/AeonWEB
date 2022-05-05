using System.Threading.Tasks;

namespace Aeon.WindowsClient;

class AeonGeneral : ServerConnection
{
	public AeonGeneral(string? token, string url) : base(token, $"{url}/aeon") { }

	public async Task<AccountInfo> GetAccountInfo() => await Request<AccountInfo>("GetAccountInfo");
}
