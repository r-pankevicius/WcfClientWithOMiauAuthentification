using Castle.DynamicProxy;
using LazyCatConsole.LazyCatServiceReference;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace LazyCatConsole
{
	/// <summary>
	/// Creates Lazy Cat Service clients.
	/// </summary>
	static class LazyCatClientFactory
	{
		static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

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
		/// Creates "level 1" manually handcrafted WCF client that supports OMiau authorization.
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
		/// Creates "level 2" dynamically generated WCF client that supports OMiau authorization.
		/// </summary>
		public static ILazyCatServiceSlimClient CreateOMiauAuthSlimClient(string endpointUrl)
		{
			var tokenService = new OMiauTokenService(endpointUrl);
			return CreateOMiauAuthSlimClient(endpointUrl, tokenService);
		}

		/*public static ILazyCatServiceSlimClient CreateOMiauAuthSlimClient(
			string endpointUrl, ITokenService tokenService)
		{
			// Create a real service client, Anonymous auth
			var realClient = CreateAnonymousAuthClient(endpointUrl);

			// Create a dispatcher for real client
			var dispatcher = new SlimServiceClientDispatcher<ILazyCatService>(realClient);

			// Generate real "object" of ILazyCatServiceSlimClient that will delegate calls
			// to the dispatcher
			var wrappedDispatcher = (ILazyCatServiceSlimClient)ProxyGenerator.CreateInterfaceProxyWithoutTarget(
				typeof(ILazyCatServiceSlimClient),
				dispatcher);

			// Create async interceptor
			var asyncInterceptor = new SlimServiceClientAsyncInterceptor<ILazyCatServiceSlimClient>(
				wrappedDispatcher, () => realClient.InnerChannel, tokenService);

			// Glue everything together
			var wrappedSlimClient = ProxyGenerator.CreateInterfaceProxyWithTargetInterface(
				wrappedDispatcher, asyncInterceptor);

			return wrappedSlimClient;
		}*/

		public static ILazyCatServiceSlimClient CreateOMiauAuthSlimClient(
			string endpointUrl, ITokenService tokenService)
		{
#warning TODO: needs redesign
			throw new NotImplementedException();
		}
	}
}
