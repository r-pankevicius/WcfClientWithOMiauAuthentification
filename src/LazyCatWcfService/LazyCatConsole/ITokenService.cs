using System.Threading.Tasks;

namespace LazyCatConsole
{
	/// <summary>
	/// Minimal abstraction of token service. The implementation may return OMiau, OAuth2, etc. tokens.
	/// </summary>
	public interface ITokenService
	{
#warning Add param for the caller specify when token needs to be refreshed (cached one did not work)
		Task<string> GetTokenAsync();
	}
}