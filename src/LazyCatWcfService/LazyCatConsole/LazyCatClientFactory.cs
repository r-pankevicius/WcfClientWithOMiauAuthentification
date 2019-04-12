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

		public static ILazyCatServiceSlimClient CreateOMiauAuthSlimClient(
			string endpointUrl, ITokenService tokenService)
		{
			// Create a real service client over OMiau channel
			var uri = new Uri(endpointUrl);
			var binding = Helpers.MakeSoap11BindingWithAnonymousAuth(uri);
			var endpointAddress = new EndpointAddress(uri);
			var realClient =
				new LazyCatServiceClientOnOMiauChannel(binding, endpointAddress, tokenService);

			var asyncInterceptor = new AngryCatAsyncInterceptor<ILazyCatService>(realClient);

			// Glue everything together
			var wrappedSlimClient = (ILazyCatServiceSlimClient)
				ProxyGenerator.CreateInterfaceProxyWithTargetInterface(
					typeof(ILazyCatServiceSlimClient), realClient, asyncInterceptor);

			return wrappedSlimClient;
		}
	}
}
