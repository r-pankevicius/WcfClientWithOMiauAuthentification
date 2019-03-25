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

			// Create a real service client, Anonymous auth
			var realClient = CreateAnonymousAuthClient(endpointUrl);

			// Intercept slim client methods - service interface methods + IDisposable methods
			var interceptor = new SlimServiceClientInterceptor<ILazyCatService>(realClient, tokenService);

			var wrappedClient = (ILazyCatServiceSlimClient)ProxyGenerator.CreateInterfaceProxyWithoutTarget(
				typeof(ILazyCatServiceSlimClient),
				interceptor);

			return wrappedClient;
		}

		#region Internals

		class OMiauTokenService : ITokenService
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
				using (var client = CreateAnonymousAuthClient(m_EndpointUrl))
				{
					string token = await client.GetOMiauToken_WithClientCredentialsAsync(CatId, CatSecret);
					return token;
				}
			}
		}

		/// <summary>
		/// The capture to be invoked by codegen when WCF service method
		/// is called.
		/// At invocation of the method we will append HTTP header needed for OMiau (or OAuth2):
		/// Authorization: Bearer super-secret-token
		/// </summary>
		/// <typeparam name="TChannel"></typeparam>
		class SlimServiceClientInterceptor<TChannel> : IInterceptor where TChannel : class
		{
			ClientBase<TChannel> m_RealClient;
			ITokenService m_TokenService;

			public SlimServiceClientInterceptor(ClientBase<TChannel> realClient, ITokenService tokenService)
			{
				m_RealClient = realClient;
				m_TokenService = tokenService;
			}

			public void Intercept(IInvocation invocation)
			{
				Type targetType = invocation.Method.DeclaringType;
				Type realType = m_RealClient.GetType();
				bool assignable = targetType.IsAssignableFrom(realType);
				if (!assignable)
				{
					throw new InvalidProgramException("Something went wrong :(");
				}

				Type realClientInterface = realType.GetInterface(targetType.FullName);
				MethodInfo[] realMethods = realClientInterface.GetMethods();
				var methodToInvoke = realMethods.First(m => m.Name.Equals(invocation.Method.Name));

				if (realClientInterface.Equals(typeof(TChannel)))
				{
					// Here we have captured a call to the service.
					// It's not needed for OMiau to apply token for each method, but
					// for OAuth2 every method in the service is secured the same way, so let's
					// get token and set Authorization header.

					//
					// If there are issues with async, wrap this wrapper again with
					// help of Castle.Core.AsyncInterceptor
					// (Double wrapped codegen with codegen-ed service channel:
					// mind blowing triple wrapper.)

					string accessToken = Task.Run(async () => await m_TokenService.GetTokenAsync()).Result;

					using (var scope = new OperationContextScope(m_RealClient.InnerChannel))
					{
						var httpRequestProperty = new HttpRequestMessageProperty();
						httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] =
							$"Bearer {accessToken}";
						OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] =
							httpRequestProperty;

						var methodResult = methodToInvoke.Invoke(m_RealClient, invocation.Arguments);
						invocation.ReturnValue = methodResult;
					}
				}
				else
				{
					var methodResult = methodToInvoke.Invoke(m_RealClient, invocation.Arguments);
					invocation.ReturnValue = methodResult;
				}
			}
		}

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
