using System.Threading.Tasks;

namespace LazyCatConsole
{
	internal class OMiauTokenService : ITokenService
	{
		const string CatId = "KOT VASYA";
		const string CatSecret = "UNDER SOFA";

		string m_EndpointUrl;

		public OMiauTokenService(string endpointUrl)
		{
			m_EndpointUrl = endpointUrl;
		}

		public async Task<string> GetTokenAsync()
		{
			using (var client = LazyCatClientFactory.CreateAnonymousAuthClient(m_EndpointUrl))
			{
				string token = await client.GetOMiauToken_WithClientCredentialsAsync(CatId, CatSecret);
				return token;
			}
		}
	}
}
