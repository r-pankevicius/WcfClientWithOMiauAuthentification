using Castle.DynamicProxy;
using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace LazyCatConsole
{
	static class Helpers
	{
		internal static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

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
		public static Binding MakeSoap11BindingWithAnonymousAuth(Uri uri)
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

		/// <summary>
		/// Creates dynamically generated slim WCF client that supports OMiau authorization.
		/// </summary>
		/// <typeparam name="TChannel">Service interface</typeparam>
		/// <typeparam name="TSlimServiceClient">
		/// Slim client interface (must derive from <typeparamref name="TChannel"/>)</typeparam>
		/// <param name="realClient">Real service client</param>
		/// <param name="tokenService">Token service</param>
		/// <returns>Slim WCF client</returns>
		public static TSlimServiceClient CreateSlimClient<TChannel, TSlimServiceClient>(
			ClientBase<TChannel> realClient, ITokenService tokenService)
			where TChannel : class
			where TSlimServiceClient : class
		{
			if (realClient == null)
				throw new ArgumentNullException(nameof(realClient));
			if (tokenService == null)
				throw new ArgumentNullException(nameof(tokenService));
			if (!typeof(TChannel).IsAssignableFrom(typeof(TSlimServiceClient)))
				throw new ArgumentNullException("Type arguments don't match.");

			// Create a dispatcher for real client
			var dispatcher = new SlimServiceClientDispatcher<TChannel>(realClient);

			// Generate a "real object" of ILazyCatServiceSlimClient that will delegate calls
			// to the dispatcher
			var wrappedDispatcher = (TSlimServiceClient)ProxyGenerator.CreateInterfaceProxyWithoutTarget(
				typeof(TSlimServiceClient),
				dispatcher);

			// Create async interceptor
			var asyncInterceptor = new SlimServiceClientAsyncInterceptor<TSlimServiceClient>(
				wrappedDispatcher, () => realClient.InnerChannel, tokenService);

			// Glue everything together to get mind blowing wrapper Bacon Double Cheeseburger
			var wrappedSlimClient = ProxyGenerator.CreateInterfaceProxyWithTargetInterface(
				wrappedDispatcher, asyncInterceptor);

			return wrappedSlimClient;
		}

		public static bool CouldBeExpiredTokenException(Exception ex)
		{
			// Very primitive exception handling, but good enough for Lazy Cats Studio
			return ex is FaultException fe && fe.Message == "🔒 Unrecognized Bearer.";
		}
	}
}
