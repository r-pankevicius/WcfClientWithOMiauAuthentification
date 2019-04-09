using Castle.DynamicProxy;
using LazyCatConsole.LazyCatServiceReference;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
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
				MakeSoap11BindingWithAnonymousAuth(uri), new EndpointAddress(uri));
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
			return new LazyCatServiceOMiauManualClient(
				MakeSoap11BindingWithAnonymousAuth(uri), new EndpointAddress(uri), tokenService);
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
		}

		#region Implementation

		//
		// About MessageVersion.Soap11 vs MessageVersion.Soap12
		//
		// The remote server returned an error: (415) Cannot process the message because the content type
		// 'application/soap+xml; charset=utf-8; action="http://tempuri.org/ISleepingCatService/SmartMathSum"'
		// was not the expected type 'text/xml; charset=utf-8'
		// https://stackoverflow.com/a/8250304
		// ... this is usually a mismatch in the client/server bindings,
		// where the message version in the service uses SOAP 1.2 (which expects application/soap+xml)
		// and the version in the client uses SOAP 1.1 (which sends text/xml).
		// WSHttpBinding uses SOAP 1.2, BasicHttpBinding uses SOAP 1.1.
		//
		static Binding MakeSoap11BindingWithAnonymousAuth(Uri uri)
		{
			BindingElement bindingElement;

			if (uri.Scheme == "http")
			{
				bindingElement = new HttpTransportBindingElement
				{
					AuthenticationScheme = AuthenticationSchemes.Anonymous,
					MaxReceivedMessageSize = 2147483647,
					MaxBufferPoolSize = 2147483647,
					MaxBufferSize = 2147483647
				};
			}
			else if (uri.Scheme == "https")
			{
				bindingElement = new HttpsTransportBindingElement
				{
					AuthenticationScheme = AuthenticationSchemes.Anonymous,
					MaxReceivedMessageSize = 2147483647,
					MaxBufferPoolSize = 2147483647,
					MaxBufferSize = 2147483647
				};
			}
			else
			{
				throw new ArgumentException($"😮 I don't know what to do with URI scheme {uri.Scheme}");
			}

			var binding = new CustomBinding(
				new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8),
				bindingElement);

			return binding;
		}

		#endregion
	}
}
