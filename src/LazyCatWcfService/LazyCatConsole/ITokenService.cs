using System.Threading.Tasks;

namespace LazyCatConsole
{
	/// <summary>
	/// Minimal abstraction of token service. The implementation may return OMiau, OAuth2, etc. tokens.
	/// </summary>
	public interface ITokenService
	{
		Task<string> GetTokenAsync();
	}
}