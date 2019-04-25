using System.Threading.Tasks;

namespace LazyCatConsole
{
	internal class OMiauTokenService : ITokenService
	{
		const string CatId = "KOT VASYA";
		const string CatSecret = "UNDER SOFA";

		readonly string m_EndpointUrl;
		string m_CachedToken;

		public OMiauTokenService(string endpointUrl)
		{
			m_EndpointUrl = endpointUrl;
		}

		public async Task<string> GetTokenAsync(bool refreshNeeded = false)
		{
			if (refreshNeeded || m_CachedToken == null)
			{
				using (var client = LazyCatClientFactory.CreateAnonymousAuthClient())
				{
					m_CachedToken = await client.GetOMiauToken_WithClientCredentialsAsync(CatId, CatSecret);
				}
			}

			return m_CachedToken;
		}
	}
}
