using Castle.DynamicProxy;
using LazyCatConsole.LazyCatServiceReference;
using System;
using System.ServiceModel;

namespace LazyCatConsole
{
	/// <summary>
	/// Creates Lazy Cat Service clients.
	/// </summary>
	static class LazyCatClientFactory
	{
		/// <summary>
		/// Creates a standard generated WCF client, Anonymous authorization.
		/// </summary>
		public static LazyCatServiceClient CreateAnonymousAuthClient(string endpointUrl)
		{
			var uri = new Uri(endpointUrl);
			return new LazyCatServiceClient(
				Helpers.MakeSoap11BindingWithAnonymousAuth(uri), new EndpointAddress(uri));
		}

		/// <summary>
		/// Creates manually handcrafted WCF client that supports OMiau authorization.
		/// </summary>
		public static LazyCatServiceOMiauManualClient CreateOMiauAuthClient(string endpointUrl)
		{
			var tokenService = new OMiauTokenService(endpointUrl);
			return CreateOMiauAuthClient(endpointUrl, tokenService);
		}

		public static LazyCatServiceOMiauManualClient CreateOMiauAuthClient(
			string endpointUrl, ITokenService tokenService)
		{
			var uri = new Uri(endpointUrl);
			var client = new LazyCatServiceOMiauManualClient(
				Helpers.MakeSoap11BindingWithAnonymousAuth(uri), new EndpointAddress(uri), tokenService);
			return client;
		}

		/// <summary>
		/// Creates dynamically generated slim WCF client that supports OMiau authorization.
		/// </summary>
		public static ILazyCatServiceSlimClient CreateOMiauAuthSlimClient(string endpointUrl)
		{
			var tokenService = new OMiauTokenService(endpointUrl);
			return CreateOMiauAuthSlimClient(endpointUrl, tokenService);
		}

		/// <summary>
		/// Creates dynamically generated slim WCF client that supports OMiau authorization.
		/// </summary>
		public static ILazyCatServiceSlimClient CreateOMiauAuthSlimClient(
			string endpointUrl, ITokenService tokenService)
		{
			var realClient = CreateAnonymousAuthClient(endpointUrl);
			var slimClient = Helpers.CreateSlimClient<ILazyCatService, ILazyCatServiceSlimClient>(
				realClient, tokenService);
			return slimClient;
		}

		//// Level 3 TODO
		//public static LazyCatServiceClient CreateLevel3Client(string endpointUrl)
		//{
		//}
	}
}
